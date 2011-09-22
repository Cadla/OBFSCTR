using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Steps;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;
using Obfuscator.Utils;
using System.Diagnostics;
using Obfuscator.Reflection;

namespace Obfuscator.Steps.Reflection
{
    public class ReplaceMemberNameStringsStep : BaseStep
    {
        static HashSet<string> SupportedMethodNames;

        static ReplaceMemberNameStringsStep()
        {
            SupportedMethodNames = new HashSet<string>()
            {
                "GetEvent",
                "GetField",
                "GetMember",
                "GetMethod",
                "GetNestedType",
                "GetProperty",
                "GetType"
            };
        }

        static bool IsSupported(MethodReference method)
        {
            if (method.DeclaringType.FullName != "System.Type")
                return false;

            if (!method.HasParameters || method.Parameters.Count > 2)
                return false;

            return true;
        }

        static bool IsGetType(MethodReference method)
        {
            if (!IsSupported(method))
                return false;

            if (method.Name != "GetType")
                return false;

            if (method.Parameters.Count == 1 && method.Parameters[0].ParameterType.FullName == "System.String")
                return true;

            return false;
        }

        static bool IsGetMember(MethodReference method)
        {
            if (!IsSupported(method) || method.HasThis == false)
                return false;

            if (method.Parameters.Count == 1 && method.Parameters[0].ParameterType.FullName == "System.String")
                return true;

            return false;
        }

        static bool IsGetMemberWithParameters(MethodReference method)
        {
            if (!IsSupported(method) || method.HasThis == false)
                return false;

            if (method.Parameters.Count != 2)
                return false;

            if (method.Parameters[0].ParameterType.FullName == "System.String" && method.Parameters[1].ParameterType.FullName == "System.Type[]")
                return true;

            return false;
        }

        protected override void ProcessAssembly(Mono.Cecil.AssemblyDefinition assembly)
        {
            var types = assembly.MainModule.GetTypes();
            var methods = assembly.MainModule.GetMemberReferences().Where(m => m is MethodReference).Cast<MethodReference>();

            var supportedMethods = new Dictionary<MethodReference, ProxyType>();
            foreach (var method in methods)
            {
                if (IsGetType(method))
                    supportedMethods.Add(method, ProxyType.GetType);
                else if (IsGetMember(method))
                    supportedMethods.Add(method, ProxyType.GetMember);
                else if (IsGetMemberWithParameters(method))
                    supportedMethods.Add(method, ProxyType.GetMemberWithParameters);
            }

            var resourcesManger = methods.SingleOrDefault(m => m.DeclaringType.FullName == "System.Resources.ResourceManager" && m.Name == ".ctor" && m.HasParameters && m.Parameters[0].ParameterType.FullName == "System.String");
            if (resourcesManger != null)
                supportedMethods.Add(resourcesManger, ProxyType.GetType);

            var map = MapInjector.InjectMap(assembly);
            MapInjector.FillMap(map, Context.DefinitionsRenameMap[assembly.Name], Context.Options.HasFlag(ObfuscationOptions.KeepNamespaces));

            //TODO: refactor - hardcodes strings
            var getType = map.Methods.Single(m => m.Name == "GetType");
            var getMember = map.Methods.Single(m => m.Name == "GetMember");
            var getMemberWithParameters = map.Methods.Single(m => m.Name == "GetMemberWithParameters");

            foreach (var type in types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;

                    var body = method.Body;
                    var closures = GetLoadByNameInstructions(body, supportedMethods);
                    if (closures.Count == 0)
                        continue;

                    ILProcessor processor = body.GetILProcessor();
                    foreach (var closure in closures)
                    {
                        var proxyType = supportedMethods[closure.method];
                        switch (proxyType)
                        {
                            case ProxyType.GetType:
                                processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Call, getType));
                                break;
                            case ProxyType.GetMember:
                                processor.InsertAfter(closure.thisLoad, processor.Create(OpCodes.Dup));
                                processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Call, getMember));
                                break;
                            case ProxyType.GetMemberWithParameters:
                                Queue<Instruction> loadSecondParameter = MethodCallClosure.LoadSecondParameterInstructions(closure);
                                processor.InsertBefore(closure.methodCall,
                                    processor.Create(OpCodes.Call, getMemberWithParameters));
                                foreach (var x in loadSecondParameter)
                                    processor.InsertBefore(closure.methodCall, x);
                                processor.InsertAfter(closure.thisLoad, processor.Create(OpCodes.Dup));
                                break;
                        }
                    }
                    body.SimplifyMacros();
                    body.OptimizeMacros();
                }
            }
        }

        private HashSet<MethodCallClosure> GetLoadByNameInstructions(MethodBody body, Dictionary<MethodReference, ProxyType> supportedMethods)
        {
            var closures = new HashSet<MethodCallClosure>();
            foreach (var instruction in body.Instructions)
            {
                var methodReference = instruction.Operand as MethodReference;
                if (methodReference != null)
                {
                    if (supportedMethods.ContainsKey(methodReference))
                    {
                        var methodCalClosure = GetMethodClosure(instruction, methodReference);
                        closures.Add(methodCalClosure);
                    }
                }
                //if (instruction.OpCode == OpCodes.Ldstr)
                //    loadInstructions.Add(instruction);
            }
            return closures;
        }

        static MethodCallClosure GetMethodClosure(Instruction methodCall, MethodReference methodReference)
        {
            var parametersCount = methodReference.Parameters.Count;

            MethodCallClosure closure = new MethodCallClosure();
            closure.method = methodReference;
            closure.methodCall = methodCall;
            closure.parameters = new Instruction[parametersCount];
            closure.opCode = methodCall.OpCode;

            var paramCount = parametersCount +  1;  // +1 is for 'this'
            var paramsToLoad = parametersCount;

    
            var current = methodCall;
            while (--paramCount != 0)
            {
                current = current.Previous;

                if (paramCount == paramsToLoad)
                    closure.parameters[--paramsToLoad] = current;

                int stackModifier = GetPositiveModifier(current.OpCode) - GetNegativeModifier(current.OpCode);
                if (stackModifier < 1)
                    paramCount += stackModifier * (-1) + 1;
                if (stackModifier > 1)
                    paramCount -= stackModifier;
            }

            if (methodReference.HasThis == false)
                closure.thisLoad = current;
            else
                closure.thisLoad = current.Previous;
            return closure;
        }

        static int GetNegativeModifier(OpCode opCode)
        {
            switch (opCode.StackBehaviourPop)
            {
                case StackBehaviour.Pop0:
                    return 0;
                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                case StackBehaviour.Varpop:
                    return 1;
                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                    return 2;
                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return 3;
                case StackBehaviour.PopAll:
                    return 1000;
            }
            return 0;
        }

        static int GetPositiveModifier(OpCode opCode)
        {
            switch (opCode.StackBehaviourPush)
            {
                case StackBehaviour.Push0:
                    return 0;
                case StackBehaviour.Push1:
                case StackBehaviour.Pushi:
                case StackBehaviour.Pushi8:
                case StackBehaviour.Pushr4:
                case StackBehaviour.Pushr8:
                case StackBehaviour.Pushref:
                case StackBehaviour.Varpush:
                    return 1;
                case StackBehaviour.Push1_push1:
                    return 2;
            }
            return 0;
        }

        class MethodCallClosure
        {
            public MethodReference method;
            public Instruction[] parameters;
            public Instruction methodCall;
            public Instruction thisLoad;
            public OpCode opCode;

            public static Queue<Instruction> LoadSecondParameterInstructions(MethodCallClosure closure)
            {
                var loadSecondParameter = new Queue<Instruction>();
                var current = closure.parameters[0];
                while (current != closure.parameters[1])
                {
                    current = current.Next;
                    loadSecondParameter.Enqueue(current);
                }
                return loadSecondParameter;
            }
        }
    }
}

