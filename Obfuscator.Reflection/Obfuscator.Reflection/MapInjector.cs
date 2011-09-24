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
            resolver.Action = delegate(TypeReference typeReference)
            {
                if (typeReference == mapTypeDefinition)
                    return module.InjectType(typeReference.Resolve(), resolver);
                else
                    return module.Import(typeReference);
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
                    string declaringTypeNewName = GetDeclaringTypeNewName(renameData, member.DeclaringType);
                    if (member.DeclaringType.IsNested)
                    {
                        var type = member.DeclaringType.DeclaringType;
                        do
                        {
                            declaringTypeNewName = GetDeclaringTypeNewName(renameData, type) + '+' + declaringTypeNewName;
                            type = type.DeclaringType;
                        } while (type != null);
                    }

                    if (keepNamespaces)
                        declaringTypeNewName = member.DeclaringType.Namespace + '.' + declaringTypeNewName;

                    if (member is MethodReference)
                        procedure = AddMemberWithParameters((MethodReference)member, newName, declaringTypeNewName, addMemberWithParameters);
                    else if (member is PropertyReference)
                        procedure = AddMemberWithParameters((PropertyReference)member, newName, declaringTypeNewName, addMemberWithParameters);
                    else
                    {
                        procedure = AddMember(member, newName, declaringTypeNewName, addMember);
                        //if (member is TypeDefinition)
                        //    AddNestedType((TypeDefinition)member, newName, declaringTypeNewName, addType, keepNamespaces);
                    }
                }
                else
                    procedure = AddType((TypeDefinition)member, newName, addType, keepNamespaces);

                foreach (var instruction in procedure)
                    processor.InsertBefore(last, instruction);
            }
        }

        private static string GetDeclaringTypeNewName(IDictionary<IMemberDefinition, string> renameData, TypeDefinition member)
        {
            string newName;
            if (!renameData.TryGetValue(member, out newName))
                newName = member.Name;
            return newName;
        }

        static Stack<Instruction> AddType(TypeDefinition type, string newName, MethodReference addType, bool keepNamespaces)
        {
            var fullNewName = newName;
            if (keepNamespaces)
                fullNewName = type.Namespace + '.' + fullNewName;

            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addType));
            result.Push(Instruction.Create(OpCodes.Ldstr, fullNewName));
#if HASH
            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMd5Hash(type.FullName)));
#else
            result.Push(Instruction.Create(OpCodes.Ldstr, type.FullName));
#endif
            return result;
        }

        //        static Stack<Instruction> AddNestedType(TypeDefinition type, string newName, string declaringTypeNewName,
        //            MethodReference addType, bool keepNamespaces)
        //        {
        //            var fullNewName = declaringTypeNewName + '+' + newName;
        //            if (keepNamespaces)
        //                fullNewName = type.Namespace + '.' + fullNewName;

        //            var result = new Stack<Instruction>();
        //            result.Push(Instruction.Create(OpCodes.Nop));
        //            result.Push(Instruction.Create(OpCodes.Call, addType));
        //            result.Push(Instruction.Create(OpCodes.Ldstr, fullNewName));
        //#if HASH
        //            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMd5Hash(type.FullName)));
        //#else
        //            result.Push(Instruction.Create(OpCodes.Ldstr, type.FullName));
        //#endif
        //            return result;
        //        }

        static Stack<Instruction> AddMember(IMemberDefinition member, string newName, string declaringTypeNewName,
            MethodReference addMember)
        {
            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addMember));
            result.Push(Instruction.Create(OpCodes.Ldstr, newName));
#if HASH
            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMd5Hash(member.Name)));
#else
            result.Push(Instruction.Create(OpCodes.Ldstr, member.Name));
#endif
            result.Push(Instruction.Create(OpCodes.Ldstr, declaringTypeNewName));
            return result;
        }


        static Stack<Instruction> AddMemberWithParameters(MethodReference method, string newName, string declaringTypeNewName,
            MethodReference addMemberWithParameters)
        {
            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addMemberWithParameters));
            result.Push(Instruction.Create(OpCodes.Ldstr, newName));
            result.Push(Instruction.Create(OpCodes.Ldstr, GetParametersString((MethodReference)method)));
#if HASH
            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMd5Hash(member.Name)));
#else
            result.Push(Instruction.Create(OpCodes.Ldstr, method.Name));
#endif
            result.Push(Instruction.Create(OpCodes.Ldstr, declaringTypeNewName));
            return result;
        }

        static Stack<Instruction> AddMemberWithParameters(PropertyReference method, string newName, string declaringTypeNewName,
    MethodReference addMemberWithParameters)
        {
            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addMemberWithParameters));
            result.Push(Instruction.Create(OpCodes.Ldstr, newName));
            result.Push(Instruction.Create(OpCodes.Ldstr, GetParametersString((PropertyReference)method)));
#if HASH
            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMd5Hash(member.Name)));
#else
            result.Push(Instruction.Create(OpCodes.Ldstr, method.Name));
#endif
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

        static string GetParametersString(PropertyReference property)
        {
            if (property.Parameters.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append('(');
                foreach (var type in property.Parameters)
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
