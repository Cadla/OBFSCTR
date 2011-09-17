using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;
using Mono.Cecil.Cil;

// NOTE umozliwienie kopiowania osobno: pol, metod typow moze prowadzic do naruszania ograniczen (internal) i ciagnie za soba cala fale roznych konsekwencji
// np. skopiowanie metody korzystajacej z prywatnych pol w klasie prowadzi do bledow itp.
namespace Obfuscator.MetadataBuilder.Extensions
{
    public static class TypeDefinitionExtensions
    {
        public static TypeDefinition InjectNestedType(this TypeDefinition targetType, TypeDefinition sourceType, ReferenceResolver resolver)
        {
            TypeDefinition newType = null;
            if (Helper.TryGetType(targetType.NestedTypes, sourceType, ref newType))
                return newType;
   
            newType = MetadataBuilderHelper.CreateNewType(sourceType, resolver);
            newType.DeclaringType = targetType;
            targetType.NestedTypes.Add(newType);

            newType.InjectTypeMembers(sourceType, resolver);
            return newType;            
        }

        public static TypeDefinition InjectTypeMembers(this TypeDefinition type, TypeDefinition sourceType, ReferenceResolver resolver)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            if (resolver == null)
                throw new ArgumentNullException("resolver");           
            
            foreach (var nestedType in sourceType.NestedTypes)
                type.InjectNestedType(nestedType, resolver);

            TypeReference x = null;
            foreach (var @interface in sourceType.Interfaces)
                if (Helper.TryGetTypeReference(type.Interfaces, @interface, ref x))
                    type.Interfaces.Add(resolver.ReferenceType(@interface, type));
            

            foreach (var field in sourceType.Fields)
                type.InjectField(field, resolver);

            foreach (var @event in sourceType.Events)
                type.InjectEvent(@event, resolver);

            foreach (var method in sourceType.Methods)
                type.InjectMethod(method, resolver);

            foreach (var property in sourceType.Properties)
                type.InjectProperty(property, resolver);

            MetadataBuilderHelper.CopySecurityDeclarations(sourceType, type, resolver);

            Console.WriteLine("Created type {0}", type.FullName);
            return type;
        }

        public static FieldDefinition InjectField(this TypeDefinition targetType, FieldDefinition sourceField, ReferenceResolver resolver)
        {
            if (sourceField == null)
                throw new ArgumentNullException("sourceField");
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            FieldDefinition newField = null;
            if (Helper.TryGetField(targetType.Fields, sourceField, ref newField))
                return newField;

            TypeReference fieldType = resolver.ReferenceType(sourceField.FieldType, targetType);
            newField = new FieldDefinition(sourceField.Name, sourceField.Attributes, fieldType)
            {
                InitialValue = sourceField.InitialValue,
                DeclaringType = targetType,
            };

            targetType.Fields.Add(newField);

            MetadataBuilderHelper.CopyCustomAttributes(sourceField, newField, resolver);

            if (newField.HasDefault)
                newField.Constant = sourceField.Constant;

            return newField;
        }

        public static EventDefinition InjectEvent(this TypeDefinition targetType, EventDefinition sourceEvent, ReferenceResolver resolver)
        {
            if (sourceEvent == null)
                throw new ArgumentNullException("sourceEvent");
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            var eventType = resolver.ReferenceType(sourceEvent.EventType, targetType);

            EventDefinition newEvent = null;
            if (Helper.TryGetEvent(targetType.Events, sourceEvent, ref newEvent))
                return newEvent;

            newEvent = new EventDefinition(sourceEvent.Name, sourceEvent.Attributes, eventType)
            {
                DeclaringType = targetType,
            };

            targetType.Events.Add(newEvent);

            MetadataBuilderHelper.CopyCustomAttributes(sourceEvent, newEvent, resolver);

            if (sourceEvent.AddMethod != null)
            {
                newEvent.AddMethod = targetType.InjectMethod(sourceEvent.AddMethod, resolver);
            }

            if (sourceEvent.RemoveMethod != null)
            {
                newEvent.RemoveMethod = targetType.InjectMethod(sourceEvent.RemoveMethod, resolver);
            }

            foreach (var otherMethod in sourceEvent.OtherMethods)
            {
                newEvent.OtherMethods.Add(targetType.InjectMethod(otherMethod, resolver));
            }

            return newEvent;
        }

        public static PropertyDefinition InjectProperty(this TypeDefinition targetType, PropertyDefinition sourceProperty, ReferenceResolver resolver)
        {
            if (sourceProperty == null)
                throw new ArgumentNullException("sourceProperty");
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            TypeReference propertyType = resolver.ReferenceType(sourceProperty.PropertyType, targetType);

            PropertyDefinition newProperty = null;
            if (Helper.TryGetProperty(targetType.Properties, sourceProperty, ref newProperty))
                return newProperty;

            newProperty = new PropertyDefinition(sourceProperty.Name, sourceProperty.Attributes, propertyType)
            {
                DeclaringType = targetType
            };
            targetType.Properties.Add(newProperty);

            MetadataBuilderHelper.CopyCustomAttributes(sourceProperty, newProperty, resolver);

            if (sourceProperty.GetMethod != null)
            {
                newProperty.GetMethod = targetType.InjectMethod(sourceProperty.GetMethod, resolver);
            }

            if (sourceProperty.SetMethod != null)
            {
                newProperty.SetMethod = targetType.InjectMethod(sourceProperty.SetMethod, resolver);
            }

            return newProperty;
        }

        public static MethodDefinition InjectMethod(this TypeDefinition targetType, MethodDefinition sourceMethod, ReferenceResolver resolver, bool body = true)
        {            
            if (sourceMethod == null)
                throw new ArgumentNullException("sourceMethod");
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            // Console.WriteLine("\tCreating method {0}", sourceMethod.FullName);

            MethodDefinition newMethod = null;
            if (Helper.TryGetMethod(targetType.Methods, sourceMethod, ref newMethod))
                return newMethod;

            newMethod = new MethodDefinition(sourceMethod.Name, sourceMethod.Attributes, sourceMethod.ReturnType)
            {
                ExplicitThis = sourceMethod.ExplicitThis,
                ImplAttributes = sourceMethod.ImplAttributes,
                SemanticsAttributes = sourceMethod.SemanticsAttributes,
                DeclaringType = targetType,
                CallingConvention = sourceMethod.CallingConvention,
            };

            targetType.Methods.Add(newMethod);

            MetadataBuilderHelper.CopyGenericParameters(sourceMethod, newMethod, resolver);

            CopyParameters(sourceMethod, newMethod, resolver);

            newMethod.ReturnType = resolver.ReferenceType(sourceMethod.ReturnType, newMethod, targetType);

            MetadataBuilderHelper.CopyCustomAttributes(sourceMethod, newMethod, resolver);

            CopyOverrides(sourceMethod, newMethod, resolver);

            MetadataBuilderHelper.CopySecurityDeclarations(sourceMethod, newMethod, resolver);

            if(body)
                CopyMethodBody(sourceMethod, newMethod, resolver);

            //  Console.WriteLine("\tCreated method {0}", sourceMethod.FullName);
            return newMethod;
        }

        private static void CopyParameters(MethodReference sourceMethod, MethodReference targetMethod, ReferenceResolver resolver)
        {
            if (!sourceMethod.HasParameters)
                return;

            //to have full method signature when determining generic patameter owner
            foreach (var parameterDefinition in sourceMethod.Parameters)
                targetMethod.Parameters.Add(parameterDefinition);

            //fix references
            for (int i = 0; i < targetMethod.Parameters.Count; i++)
            {
                var parameterDefinition = targetMethod.Parameters[i];

                var parameterType = resolver.ReferenceType(parameterDefinition.ParameterType, targetMethod, targetMethod.DeclaringType);
                ParameterDefinition newParameter = new ParameterDefinition(parameterDefinition.Name, parameterDefinition.Attributes, parameterType)
                {
                    HasConstant = parameterDefinition.HasConstant,
                    MarshalInfo = parameterDefinition.MarshalInfo,
                };
                targetMethod.Parameters[i] = newParameter;

                if (parameterDefinition.HasDefault)
                    newParameter.Constant = parameterDefinition.Constant;
                MetadataBuilderHelper.CopyCustomAttributes(parameterDefinition, newParameter, resolver);
            }
        }

        private static void CopyOverrides(MethodDefinition sourceMethod, MethodDefinition targetMethod, ReferenceResolver resolver)
        {
            if (!sourceMethod.HasOverrides)
                return;

            foreach (var methodOverride in sourceMethod.Overrides)
                targetMethod.Overrides.Add(resolver.ReferenceMethod(methodOverride, targetMethod, targetMethod.DeclaringType));
        }

        private static void CopyMethodBody(MethodDefinition sourceMethod, MethodDefinition targetMethod, ReferenceResolver resolver)
        {
            if (!sourceMethod.HasBody)
                return;

            targetMethod.Body.InitLocals = sourceMethod.Body.InitLocals;
            //targetMethod.Body.MaxStackSize = sourceMethod.Body.MaxStackSize;
            //targetMethod.Body.LocalVarToken = sourceMethod.Body.LocalVarToken;

            CopyVariables(sourceMethod, targetMethod, resolver);
            CopyInstructions(sourceMethod, targetMethod, resolver);
        }

        private static void CopyVariables(MethodDefinition sourceMethod, MethodDefinition targetMethod, ReferenceResolver resolver)
        {
            if (!sourceMethod.Body.HasVariables)
                return;

            foreach (var variableDefinition in sourceMethod.Body.Variables)
            {
                var variableType = resolver.ReferenceType(variableDefinition.VariableType, targetMethod, targetMethod.DeclaringType);
                VariableDefinition newVariable = new VariableDefinition(variableDefinition.Name, variableType);
                targetMethod.Body.Variables.Add(newVariable);
            }
        }

        private static void CopyInstructions(MethodDefinition sourceMethod, MethodDefinition targetMethod, ReferenceResolver resolver)
        {
            TypeDefinition targetType = targetMethod.DeclaringType;

            var processor = targetMethod.Body.GetILProcessor();
            var offset = 0;
            foreach (var instruction in sourceMethod.Body.Instructions)
            {
                object operand;

                if (instruction.Operand is FieldReference)
                {
                    operand = resolver.ReferenceField((FieldReference)instruction.Operand, targetMethod, targetType);
                }
                else if (instruction.Operand is MethodReference)
                {
                    operand = resolver.ReferenceMethod((MethodReference)instruction.Operand, targetMethod, targetType);
                }
                else if (instruction.Operand is TypeReference)
                {
                    operand = resolver.ReferenceType((TypeReference)instruction.Operand, targetMethod, targetType);
                }
                else
                {
                    operand = instruction.Operand;
                }

                Instruction newInstruction = Helper.CreateInstruction(instruction.OpCode, instruction.OpCode.OperandType, operand);
                newInstruction.SequencePoint = instruction.SequencePoint;

                newInstruction.Offset = offset;
                offset += newInstruction.GetSize();

                processor.Append(newInstruction);
            }

            FixBranchingTargets(targetMethod.Body);
            CopyExceptionHandlers(sourceMethod, targetMethod, resolver);
        }

        private static void FixBranchingTargets(MethodBody methodBody)
        {
            foreach (var instruction in methodBody.Instructions)
            {
                switch (instruction.OpCode.OperandType)
                {
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineBrTarget:
                        instruction.Operand = GetInstruction(methodBody, (Instruction)instruction.Operand);
                        break;
                    case OperandType.InlineSwitch:
                        var targets = (Instruction[])instruction.Operand;
                        for (int i = 0; i < targets.Length; i++)
                            targets[i] = GetInstruction(methodBody, targets[i]);
                        break;
                }
            }
        }

        private static void CopyExceptionHandlers(MethodDefinition sourceMethod, MethodDefinition targetMethod, ReferenceResolver resolver)
        {
            if (!sourceMethod.Body.HasExceptionHandlers)
                return;

            foreach (var handler in sourceMethod.Body.ExceptionHandlers)
            {
                var newHandler = new ExceptionHandler(handler.HandlerType)
                {
                    FilterStart = GetInstruction(targetMethod.Body, handler.FilterStart),
             //       FilterEnd = GetInstruction(targetMethod.Body, handler.FilterEnd),
                    HandlerStart = GetInstruction(targetMethod.Body, handler.HandlerStart),
                    HandlerEnd = GetInstruction(targetMethod.Body, handler.HandlerEnd),
                    TryStart = GetInstruction(targetMethod.Body, handler.TryStart),
                    TryEnd = GetInstruction(targetMethod.Body, handler.TryEnd),
                };

                if (handler.CatchType != null)
                    newHandler.CatchType = resolver.ReferenceType(handler.CatchType, targetMethod, targetMethod.DeclaringType);

                targetMethod.Body.ExceptionHandlers.Add(newHandler);
            }
        }

        private static Instruction GetInstruction(Mono.Cecil.Cil.MethodBody methodBody, Instruction instruction)
        {
            if (instruction == null)
                return null;

            return methodBody.Instructions.First(x => x.Offset == instruction.Offset);
        }
    }
}
