using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;
using Mono.Cecil.Cil;
using System.Diagnostics;

namespace Obfuscator.Steps
{
    public class ConverterStep : BaseStep
    {
        AssemblyVisitor _visitor;

      

        public ConverterStep()
        {
            _visitor = new AssemblyVisitor();
        }

        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            _visitor.ConductVisit(assembly, new ConverterVisitor());
        }
    }

    public partial class ConverterVisitor : NullAssemblyVisitor
    {
        IList<KeyValuePair<MethodReference, MethodCallClosure>> closure = new List<KeyValuePair<MethodReference, MethodCallClosure>>();

        public override VisitorLevel Level()
        {
            return VisitorLevel.MethodBodys;
        }

        public override void VisitTypeDefinition(Mono.Cecil.TypeDefinition type)
        {
            var runtimeName = type.FullName;
        }


        public override void VisitTypeReference(Mono.Cecil.TypeReference type)
        {

        }

        public override void VisitMethodBody(MethodBody body)
        {
            foreach (var instruction in body.Instructions)
            {
                if (instruction.OpCode != OpCodes.Call &&
                    instruction.OpCode != OpCodes.Calli &&
                    instruction.OpCode != OpCodes.Callvirt)
                    continue;

                var methodReference = instruction.Operand as MethodReference;
         

                if (methodReference != null && methodReference.HasThis)
                {
                    var closure = GetMethodClosure(instruction, methodReference);
                }
            }
        }

        private MethodCallClosure GetMethodClosure(Instruction methodCall, MethodReference methodReference)
        {
            Debug.Assert(methodReference.HasThis);

            MethodCallClosure closure = new MethodCallClosure();

            var parametersCount = methodReference.Parameters.Count;

            closure.method = methodReference;
            closure.methodCall = methodCall;
            closure.parameters = new Instruction[parametersCount];
            closure.opCode = methodCall.OpCode;

            var paramCount = parametersCount + 1;  // +1 is for 'this'
            var paramsToLoad = parametersCount;

            var current = methodCall;
            while (--paramCount != 0)
            {
                current = current.Previous;

                int stackModifier = GetPositiveModifier(current.OpCode) -  GetNegativeModifier(current.OpCode);

                if (stackModifier < 1)
                    paramCount += stackModifier * (-1) + 1;
                if (stackModifier > 1)
                    paramCount -= stackModifier;

                if (paramCount == paramsToLoad)
                    closure.parameters[--paramsToLoad] = current;
            }
            closure.thisLoad = current.Previous;
            return closure;
        }

        int GetNegativeModifier(OpCode opCode)
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

        int GetPositiveModifier(OpCode opCode)
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
            public Instruction []parameters;
            public Instruction methodCall;
            public Instruction thisLoad;
            public OpCode opCode;
        }
    }
}
