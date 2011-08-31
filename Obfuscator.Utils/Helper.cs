using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Obfuscator.Utils
{
    public static partial class Helper
    {
        public static bool IsCore(TypeReference type)
        {
            if (type.Scope != null)
            {
                return IsCoreAssemblyName(type.Scope.Name);
            }
            return false;
        }

        public static bool IsCoreAssemblyName(String name)
        {
            switch (name)
            {
                case "mscorlib":
                case "Accessibility":
                case "CommonLanguageRuntimeLibrary":
                case "Mono.Security":
                    return true;
                default:
                    return name.StartsWith("System")
                        || name.StartsWith("Microsoft");
            }
        }

        //TODO Methods below can be changed to generic probably with use of Linq
        public static TypeDefinition GetType(ModuleDefinition module, TypeReference type)
        {
            if (!type.IsNested)
                return module.GetType(type.Namespace, type.Name);

            var declaring_type = GetType(module, type.DeclaringType);
            if (declaring_type == null)
                return null;

            return GetNestedType(declaring_type, type.Name);
        }

        public static TypeDefinition GetNestedType(TypeDefinition type, string name)
        {
            if (!type.HasNestedTypes)
                return null;

            var nested_types = type.NestedTypes;

            for (int i = 0; i < nested_types.Count; i++)
            {
                var nested_type = nested_types[i];
                if (nested_type.Name == name)
                    return nested_type;
            }

            return null;
        }

        public static TypeReference GetType(IList<TypeReference> types, TypeReference reference)
        {
            foreach (var type in types)
            {
                if (!AreSame(type, reference))
                    continue;

                return type;
            }
            return null;
        }

        public static FieldDefinition GetField(IList<FieldDefinition> fields, FieldReference reference)
        {
            foreach (var field in fields)
            {
                if (!AreSame(field, reference))
                    continue;

                return field;
            }
            return null;
        }

        public static MethodDefinition GetMethod(IList<MethodDefinition> methods, MethodReference reference)
        {
            foreach (var method in methods)
            {
                if (!AreSame(method, reference))
                    continue;

                return method;
            }
            return null;
        }

        public static PropertyDefinition GetProperty(IList<PropertyDefinition> properties, PropertyReference reference)
        {
            foreach (var property in properties)
            {
                if (!AreSame(property, reference))
                    continue;

                return property;
            }
            return null;
        }

        public static EventDefinition GetEvent(IList<EventDefinition> events, EventReference reference)
        {
            foreach (var evnt in events)
            {
                if (!AreSame(evnt, reference))
                    continue;

                return evnt;
            }
            return null;
        }

        public static CustomAttribute GetCustomAttribute(IList<CustomAttribute> attributes, CustomAttribute reference)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                if (!AreSame(attribute, reference))
                    continue;

                return attribute;
            }

            return null;
        }

        public static bool AreSame(TypeReference a, TypeReference b)
        {
            if (a.MetadataType != b.MetadataType)
                return false;

            if (a.IsGenericParameter)
                return AreSame((GenericParameter)a, (GenericParameter)b);

            if (IsTypeSpecification(a))
                return AreSame((TypeSpecification)a, (TypeSpecification)b);

            return a.FullName == b.FullName;
        }

        public static bool AreSame(MethodReference a, MethodReference b)
        {
            if (a.Name != b.Name)
                return false;

            if (!AreSame(a.ReturnType, b.ReturnType))
                return false;

            if (a.HasParameters != b.HasParameters)
                return false;

            if (!a.HasParameters && !b.HasParameters)
                return true;

            if (!AreSame(a.Parameters, b.Parameters))
                return false;

            if (!a.HasGenericParameters && !b.HasGenericParameters)
                return true;

            if (a.GenericParameters.Count != b.GenericParameters.Count)
                return false;

            return a.FullName == b.FullName;
        }

        public static bool AreSame(PropertyReference a, PropertyReference b)
        {
            if (a.Name != b.Name)
                return false;

            if (!AreSame(a.PropertyType, b.PropertyType))
                return false;

            if (!AreSame(a.DeclaringType, b.DeclaringType))
                return false;

            if (!AreSame(a.Parameters, b.Parameters))
                return false;

            return true;
        }

        public static bool AreSame(FieldReference a, FieldReference b)
        {
            if (a.Name != b.Name)
                return false;

            if (!AreSame(a.FieldType, b.FieldType))
                return false;

            return true;
        }

        public static bool AreSame(EventReference a, EventReference b)
        {
            //TODO is it enough?
            if (a.Name != b.Name)
                return false;

            if (!AreSame(a.EventType, b.EventType))
                return false;
            return true;
        }

        public static bool AreSame(CustomAttribute a, CustomAttribute b)
        {
            if (!AreSame(a.Constructor, b.Constructor))
                return false;

            if (!AreSame(a.AttributeType, b.AttributeType))
                return false;

            return true;
        }

        public static bool AreSame(Collection<ParameterDefinition> a, Collection<ParameterDefinition> b)
        {
            var count = a.Count;

            if (count != b.Count)
                return false;

            if (count == 0)
                return true;

            for (int i = 0; i < count; i++)
                if (!AreSame(a[i].ParameterType, b[i].ParameterType))
                    return false;

            return true;
        }

        //public static bool AreSame(Collection<GenericParameter> a, Collection<GenericParameter> b)
        //{
        //    var count = a.Count;

        //    if(count != b.Count)
        //        return false;

        //    if(count == 0)
        //        return true;

        //    for(int i = 0; i < count; i++)
        //        if(!AreSame(a[i],b[i]))
        //            return false;

        //    return true;
        //}

        public static bool AreSame(AssemblyNameReference a, AssemblyNameReference b)
        {
            return a.FullName == b.FullName;
        }

        public static bool AreSame(TypeSpecification a, TypeSpecification b)
        {
            if (!AreSame(a.ElementType, b.ElementType))
                return false;

            if (a.IsGenericInstance)
                return AreSame((GenericInstanceType)a, (GenericInstanceType)b);

            if (a.IsRequiredModifier || a.IsOptionalModifier)
                return AreSame((IModifierType)a, (IModifierType)b);

            if (a.IsArray)
                return AreSame((ArrayType)a, (ArrayType)b);

            return true;
        }

        public static bool AreSame(ArrayType a, ArrayType b)
        {
            if (a.Rank != b.Rank)
                return false;

            // TODO: dimensions

            return true;
        }

        public static bool AreSame(IModifierType a, IModifierType b)
        {
            return AreSame(a.ModifierType, b.ModifierType);
        }

        public static bool AreSame(GenericInstanceType a, GenericInstanceType b)
        {
            if (!a.HasGenericArguments)
                return !b.HasGenericArguments;

            if (!b.HasGenericArguments)
                return false;

            if (a.GenericArguments.Count != b.GenericArguments.Count)
                return false;

            for (int i = 0; i < a.GenericArguments.Count; i++)
                if (!AreSame(a.GenericArguments[i], b.GenericArguments[i]))
                    return false;

            return true;
        }

        public static bool AreSame(GenericParameter a, GenericParameter b)
        {
            return a.Position == b.Position;
        }

        public static bool IsTypeSpecification(TypeReference type)
        {
            switch (type.MetadataType)
            {
                case MetadataType.Array:
                case MetadataType.ByReference:
                case MetadataType.OptionalModifier:
                case MetadataType.RequiredModifier:
                case MetadataType.FunctionPointer:
                case MetadataType.GenericInstance:
                case MetadataType.MVar:
                case MetadataType.Pinned:
                case MetadataType.Pointer:
                //case ElementType.SzArray:
                case MetadataType.Sentinel:
                case MetadataType.Var:
                    return true;
            }

            return false;
        }

        public static Instruction CreateInstruction(OpCode opCode, OperandType operandType, object operand)
        {
            if (opCode.Code == Code.Calli)
                return Instruction.Create(opCode, (CallSite)operand);

            switch (operandType)
            {
                case OperandType.InlineArg:
                case OperandType.ShortInlineArg:
                    return Instruction.Create(opCode, (ParameterDefinition)operand);
                case OperandType.InlineBrTarget:
                case OperandType.ShortInlineBrTarget:
                    return Instruction.Create(opCode, (Instruction)operand);
                case OperandType.InlineField:
                    return Instruction.Create(opCode, (FieldReference)operand);
                case OperandType.InlineI:
                    return Instruction.Create(opCode, (int)operand);
                case OperandType.ShortInlineI:
                    return Instruction.Create(opCode, (sbyte)operand);
                case OperandType.InlineI8:
                    return Instruction.Create(opCode, (long)operand);
                case OperandType.InlineMethod:
                    return Instruction.Create(opCode, (MethodReference)operand);
                case OperandType.InlineNone:
                    return Instruction.Create(opCode);
                case OperandType.InlineR:
                    return Instruction.Create(opCode, (double)operand);
                case OperandType.ShortInlineR:
                    return Instruction.Create(opCode, (float)operand);
                case OperandType.InlineString:
                    return Instruction.Create(opCode, (String)operand);
                case OperandType.InlineSwitch:
                    return Instruction.Create(opCode, (Instruction[])operand);
                case OperandType.InlineType:
                    return Instruction.Create(opCode, (TypeReference)operand);
                case OperandType.InlineVar:
                case OperandType.ShortInlineVar:
                    return Instruction.Create(opCode, (VariableDefinition)operand);
                case OperandType.InlineTok:
                    if (operand is TypeReference)
                        return Instruction.Create(opCode, (TypeReference)operand);
                    if (operand is MethodReference)
                        return Instruction.Create(opCode, (MethodReference)operand);
                    if (operand is FieldReference)
                        return Instruction.Create(opCode, (FieldReference)operand);
                    break;

            }
            return null;
        }

        //TODO check wheter library.dll and library.exe are different scope names char separator = type.DeclaringType.IsNested ? '/' : '.';
        public static string GetTypeScope(TypeDefinition type)
        {
            if (type.IsNested)
            {
                char separator = type.DeclaringType.IsNested ? '/' : '.';
                return GetScope(type.DeclaringType) + separator + type.DeclaringType.Name;
            }
            return type.Namespace;
        }

        public static string GetScope(IMemberDefinition member)
        {
            TypeDefinition type = member as TypeDefinition;

            if (type != null)
                return GetTypeScope(type);

            StringBuilder scope = new StringBuilder();
            TypeDefinition declaringType = member.DeclaringType;

            scope.Append(GetTypeScope(declaringType));
            char separator = member.DeclaringType.IsNested ? '/' : '.';
            scope.Append(separator);
            scope.Append(declaringType.Name);
            scope.Append("::");
            scope.Append(member.MetadataToken.TokenType.ToString());

            MethodDefinition method = member as MethodDefinition;
            if (method != null)
                scope.Append(GetMethodParametersString(method));

            return scope.ToString();
        }

        //public static string GetMethodSignatureString(MethodReference method)
        //{
        //    StringBuilder signature = new StringBuilder();
        //    signature.Append(method.Name);            
        //    signature.Append(GetMethodParametersString(method));            
        //    return signature.ToString();
        //}



        private static string GetMethodParametersString(MethodReference method)
        {
            StringBuilder list = new StringBuilder();
            if (method.HasParameters)
            {
                list.Append("(");
                foreach (var param in method.Parameters)
                {
                    list.Append(param.ParameterType.FullName + ",");
                }
                list.Remove(list.Length - 1, 1);
                list.Append(")");
            }
            return list.ToString();
        }
    }
}
