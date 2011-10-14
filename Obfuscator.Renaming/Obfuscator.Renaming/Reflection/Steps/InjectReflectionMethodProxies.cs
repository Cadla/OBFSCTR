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
using Obfuscator.Renaming.Steps;

namespace Obfuscator.Renaming.Reflection.Steps
{
    internal enum ReflectionMethodProxy
    {
        GetTypeName,
        GetEventName,
        GetFieldName,
        GetNestedTypeName,
        GetMethodName,
        GetPropertyName,
        GetTypeNameFromAssembly,
    }

    internal class InjectReflectionMethodProxies : RenamingBaseStep
    {
        ReflectionOptions _reflectionOptions;
        bool _keepNamespaces;

        public InjectReflectionMethodProxies(ReflectionOptions reflectionOptions, bool keepNamespaces)
        {
            _reflectionOptions = reflectionOptions;
        }

        protected override void ProcessAssembly(Mono.Cecil.AssemblyDefinition assembly)
        {
            var types = assembly.MainModule.GetTypes();

            var map = InjectRenameMap(assembly, Context.DefinitionsRenameMap[assembly.Name]);

            var supportedMethods = CollectSupportedMethodsReferences(assembly);
            
            InsertProxyMethods(types, supportedMethods, map);
        }

        private Dictionary<MethodReference, ReflectionMethodProxy> CollectSupportedMethodsReferences(AssemblyDefinition assembly)
        {
            var methods = assembly.MainModule.GetMemberReferences().Where(m => m is MethodReference).Cast<MethodReference>();
            var supportedMethods = new Dictionary<MethodReference, ReflectionMethodProxy>();
            foreach (var method in methods)
            {
                if (_reflectionOptions.HasFlag(ReflectionOptions.Types) &&
                    (ReflectionMethodsHelper.IsGetType(method) || ReflectionMethodsHelper.IsCreateResourceManager(method)))
                    supportedMethods.Add(method, ReflectionMethodProxy.GetTypeName);
                else if (_reflectionOptions.HasFlag(ReflectionOptions.Types) && ReflectionMethodsHelper.IsGetTypeFromAssembly(method))
                    supportedMethods.Add(method, ReflectionMethodProxy.GetTypeNameFromAssembly);
                else if (_reflectionOptions.HasFlag(ReflectionOptions.Methods) && ReflectionMethodsHelper.IsGetMethod(method))
                    supportedMethods.Add(method, ReflectionMethodProxy.GetMethodName);
                else if (_reflectionOptions.HasFlag(ReflectionOptions.Fields) && ReflectionMethodsHelper.IsGetField(method))
                    supportedMethods.Add(method, ReflectionMethodProxy.GetFieldName);
                else if (_reflectionOptions.HasFlag(ReflectionOptions.Events) && ReflectionMethodsHelper.IsGetEvent(method))
                    supportedMethods.Add(method, ReflectionMethodProxy.GetEventName);
                else if (_reflectionOptions.HasFlag(ReflectionOptions.Properties) && ReflectionMethodsHelper.IsGetProperty(method))
                    supportedMethods.Add(method, ReflectionMethodProxy.GetPropertyName);
                else if (_reflectionOptions.HasFlag(ReflectionOptions.NestedTypes) && ReflectionMethodsHelper.IsGetNestedType(method))
                    supportedMethods.Add(method, ReflectionMethodProxy.GetNestedTypeName);
            }
            return supportedMethods;
        }

        private TypeDefinition InjectRenameMap(Mono.Cecil.AssemblyDefinition assembly, IDictionary<IMemberDefinition, string> renameMap)
        {
            var map = MapInjector.InjectMap(assembly);
            MapInjector.FillMap(map, renameMap, _reflectionOptions, _keepNamespaces);
            return map;
        }

        private void InsertProxyMethods(IEnumerable<TypeDefinition> types, Dictionary<MethodReference, ReflectionMethodProxy> supportedMethods, TypeDefinition map)
        {
            var getTypeName = map.Methods.Single(m => m.Name == "GetTypeName");
            var getFieldName = map.Methods.Single(m => m.Name == "GetFieldName");
            var getEventName = map.Methods.Single(m => m.Name == "GetEventName");
            var getMetodName = map.Methods.Single(m => m.Name == "GetMethodName");
            var getPropertyName = map.Methods.Single(m => m.Name == "GetPropertyName");
            var getNestedTypeName = map.Methods.Single(m => m.Name == "GetNestedTypeName");
            var getTypeNameFromAssembly = map.Methods.Single(m => m.Name == "GetTypeNameFromAssembly");

            foreach (var type in types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;

                    var body = method.Body;
                    var closures = GetSupportedMethodCalls(body, supportedMethods);
                    if (closures.Count == 0)
                        continue;

                    ILProcessor processor = body.GetILProcessor();
                    foreach (var closure in closures)
                    {
                        var proxyType = supportedMethods[closure.methodReference];
                        //TODO: Refactor, duplicated code
                        switch (proxyType)
                        {
                            case ReflectionMethodProxy.GetTypeName:                                
                                processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Call, getTypeName));
                                break;
                            case ReflectionMethodProxy.GetFieldName:
                                processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Dup));
                                processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Call, getFieldName));
                                break;
                            case ReflectionMethodProxy.GetEventName:
                                processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Dup));
                                processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Call, getEventName));
                                break;
                            case ReflectionMethodProxy.GetNestedTypeName:
                                processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Dup));
                                processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Call, getNestedTypeName));
                                break;
                            case ReflectionMethodProxy.GetTypeNameFromAssembly:
                                processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Dup));
                                processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Call, getTypeNameFromAssembly));
                                break;
                            //case ReflectionMethodProxy.GetMethodName:
                            //    Queue<Instruction> loadSecondParameter = MethodCallStackFrame.LoadParameter(closure, 1);
                            //    processor.InsertBefore(closure.methodCall,
                            //        processor.Create(OpCodes.Call, getMetodName));
                            //    foreach (var x in loadSecondParameter)
                            //        processor.InsertBefore(closure.methodCall, x);
                            //    processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Dup));
                            //    break;
                            //case ReflectionMethodProxy.GetPropertyName:
                            //    Queue<Instruction> loadSecondParameter2 = MethodCallStackFrame.LoadParameter(closure, 1);
                            //    processor.InsertBefore(closure.methodCall,
                            //        processor.Create(OpCodes.Call, getPropertyName));
                            //    foreach (var x in loadSecondParameter2)
                            //        processor.InsertBefore(closure.methodCall, x);
                            //    processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Dup));
                            //    break;
                            case ReflectionMethodProxy.GetMethodName:                                
                                processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Call, getMetodName));
                                if (MethodCallStackFrame.IsMethodCall(closure.parameters[2].OpCode))
                                { 
                                    var m = closure.parameters[2].Operand as MethodReference;
                                    var v = new VariableDefinition(m.ReturnType);
                                    body.Variables.Add(v);
                                    processor.InsertAfter(closure.parameters[2], processor.Create(OpCodes.Ldloc_S, v));
                                    processor.InsertAfter(closure.parameters[2], processor.Create(OpCodes.Stloc_S, v));
                                    processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Ldloc_S, v));
                                }
                                else
                                    processor.InsertBefore(closure.methodCall, closure.parameters[2]);                                
                                processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Dup));
                                break;
                            case ReflectionMethodProxy.GetPropertyName:                                
                                      processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Call, getPropertyName));
                                if (MethodCallStackFrame.IsMethodCall(closure.parameters[2].OpCode))
                                { 
                                    var m = closure.parameters[2].Operand as MethodReference;
                                    var v = new VariableDefinition(m.ReturnType);
                                    body.Variables.Add(v);
                                    processor.InsertAfter(closure.parameters[2], processor.Create(OpCodes.Ldloc_S, v));
                                    processor.InsertAfter(closure.parameters[2], processor.Create(OpCodes.Stloc_S, v));
                                    processor.InsertBefore(closure.methodCall, processor.Create(OpCodes.Ldloc_S, v));
                                }
                                else
                                    processor.InsertBefore(closure.methodCall, closure.parameters[2]);                                
                                processor.InsertAfter(closure.parameters[0], processor.Create(OpCodes.Dup));
                                break;
                        }
                    }
                    // Fix offsets after adding new instructions
                    body.SimplifyMacros();
                    body.OptimizeMacros();
                }
            }
        }

        private IList<MethodCallStackFrame> GetSupportedMethodCalls(MethodBody body, Dictionary<MethodReference, ReflectionMethodProxy> supportedMethods)
        {
            var methodCalls = new List<MethodCallStackFrame>();
            foreach (var instruction in body.Instructions)
            {
                var methodReference = instruction.Operand as MethodReference;
                if (methodReference != null && supportedMethods.ContainsKey(methodReference))
                {
                    var frame = MethodCallStackFrame.GetMethodCallStackFrame(instruction);
                    methodCalls.Add(frame);
                }
            }
            return methodCalls;
        }   
    }
}



