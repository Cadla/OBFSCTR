using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Obfuscator.MetadataBuilder.Extensions;
using Obfuscator.Utils;

namespace Obfuscator.Reflection
{
    internal enum ProxyType
    {
        GetType,
        GetMember,
        GetMemberWithParameters
    }

    internal static class MapInjector
    {
        public static TypeDefinition InjectMap(AssemblyDefinition assembly)
        {
            var module = assembly.MainModule;
            var mapModule = ModuleDefinition.ReadModule(typeof(Map).Module.FullyQualifiedName);
            var mapTypeDefinition = mapModule.GetType(typeof(Map).FullName);

            string destinationName = "<PrivateImplementationDetails>{" + Guid.NewGuid().ToString().ToUpper() + "}";

            ReferenceResolver resolver = new ReferenceResolver(module, Helper.IsCore);
            resolver.Action = delegate(TypeDefinition typeDefinition)
            {
                if (typeDefinition == mapTypeDefinition)
                    return module.InjectType(typeDefinition, resolver);
                else
                    return module.Import(typeDefinition);
            };

            var newType = module.InjectType(mapTypeDefinition, resolver);

            newType.Name = destinationName;
            newType.Namespace = String.Empty;

            return newType;
        }

        public static void FillMap(TypeDefinition map, IDictionary<IMemberDefinition, string> renameData, bool keepNamespaces)
        {
            var constructor = map.Methods.Single(m => m.IsConstructor);

            //TODO: refactor - hardcodes strings
            var addType = map.Methods.Single(m => m.Name == "AddType");
            var addMember = map.Methods.Single(m => m.Name == "AddMember");
            var addMemberWithParameters = map.Methods.Single(m => m.Name == "AddMemberWithParameters");

            ILProcessor processor = constructor.Body.GetILProcessor();
            var last = processor.Body.Instructions.Single(i => i.OpCode == OpCodes.Ret);
            foreach (var entry in renameData)
            {
                var member = entry.Key;
                var newName = entry.Value;

                Stack<Instruction> procedure;
                if (member.DeclaringType != null)
                {
                    var declaringTypeNewName = member.DeclaringType.Namespace + '.' + renameData[member.DeclaringType];
                    //if (member is MethodReference && ((MethodReference)member).HasParameters)
                    //    procedure = AddMemberWithParameters(member, newName, declaringTypeNewName, addMemberWithParameters);
                    //else if (member is PropertyReference && ((PropertyReference)member).Parameters.Count != 0)
                    procedure = AddMemberWithParameters(member, newName, declaringTypeNewName, addMemberWithParameters);
                    //else
                    //    procedure = AddMember(member, newName, declaringTypeNewName, addMember);
                }
                else
                    procedure = AddType((TypeDefinition)member, newName, addType, keepNamespaces);

                foreach (var instruction in procedure)
                    processor.InsertBefore(last, instruction);
            }
        }

        static Stack<Instruction> AddType(TypeDefinition type, string newName, MethodReference addType, bool keepNamespaces)
        {
            var fullNewName = newName;
            if (keepNamespaces)
                fullNewName = type.Namespace + '.' + newName;

            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addType));
            result.Push(Instruction.Create(OpCodes.Ldstr, fullNewName));
            result.Push(Instruction.Create(OpCodes.Ldstr, type.FullName));
            return result;
        }

        static Stack<Instruction> AddMember(IMemberDefinition member, string newName, string declaringTypeNewName,
            MethodReference addMember)
        {
            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addMember));
            result.Push(Instruction.Create(OpCodes.Ldstr, newName));
            result.Push(Instruction.Create(OpCodes.Ldstr, member.Name));
            result.Push(Instruction.Create(OpCodes.Ldstr, declaringTypeNewName));
            return result;
        }


        static Stack<Instruction> AddMemberWithParameters(IMemberDefinition member, string newName, string declaringTypeNewName,
            MethodReference addMemberWithParameters)
        {
            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addMemberWithParameters));
            result.Push(Instruction.Create(OpCodes.Ldstr, newName));
            result.Push(Instruction.Create(OpCodes.Ldstr, GetParametersString((MethodReference)member)));
            result.Push(Instruction.Create(OpCodes.Ldstr, member.Name));
            result.Push(Instruction.Create(OpCodes.Ldstr, declaringTypeNewName));
            return result;
        }

        static string GetParametersString(MethodReference method)
        {
            if (method.HasParameters)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append('(');
                foreach (var type in method.Parameters)
                {
                    builder.Append(type.ParameterType.FullName);
                    builder.Append(',');
                }
                builder.Remove(builder.Length - 1, 1);
                builder.Append(')');
                return builder.ToString();
            }
            return "()";
        }
    }
}
