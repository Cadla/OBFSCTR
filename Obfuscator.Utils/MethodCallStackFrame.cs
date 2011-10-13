using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Obfuscator.Utils
{
    public class MethodCallStackFrame
    {
        public MethodReference method;
        public Instruction[] parameters;
        public Instruction methodCall;
        //public OpCode opCode;

        //public static Queue<Instruction> LoadParameter(MethodCallStackFrame closure, int index)
        //{
        //    //var loadParameter = new Queue<Instruction>();
        //    //Instruction current;
        //    ////if(index == 0)
        //    ////    current = closure.thisLoad;
        //    ////else
        //    //    current = closure.parameters[index - 1];

        //    //while (current != closure.parameters[index])
        //    //{
        //    //    current = current.Next;
        //    //    loadParameter.Enqueue(current);
        //    //}
        //    //return loadParameter;            
        //}

        public static MethodCallStackFrame GetMethodCallStackFrame(Instruction methodCall, MethodReference methodReference)
        {
            var parametersCount = methodReference.Parameters.Count + (methodReference.HasThis ? 1 : 0);

            MethodCallStackFrame closure = new MethodCallStackFrame()
            {
                method = methodReference,
                methodCall = methodCall,
                parameters = new Instruction[parametersCount]
            };

            var paramLoadingInst = parametersCount; // +1 for the method call

            var paramToLoad = parametersCount - 1;
            var current = methodCall;
            while(paramLoadingInst != 0)
            {
                current = current.Previous; 
                int stackModifier = GetPositiveModifier(current.OpCode) - GetNegativeModifier(current.OpCode);

                if(IsMethodCall(current.OpCode)){
                    var method = current.Operand as MethodReference;
                    stackModifier += (method.ReturnType.FullName == "System.Void" ? 0 : 1) - method.Parameters.Count;
                }

                if (stackModifier != 0)
                    paramLoadingInst -= stackModifier;

                //if (stackModifier >= 1)
                //    paramLoadingInst -= stackModifier;

                if (paramLoadingInst == paramToLoad)
                    closure.parameters[paramToLoad--] = current;
            }

            //if (methodReference.HasThis != false)
            //    closure.thisLoad = current.Previous;
            
            return closure;
        }

        private static bool IsMethodCall(OpCode opCode)
        {
            return opCode == OpCodes.Call || opCode == OpCodes.Calli || opCode == OpCodes.Callvirt;
        }

        private static int GetNegativeModifier(OpCode opCode)
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

        private static int GetPositiveModifier(OpCode opCode)
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
    }
}
