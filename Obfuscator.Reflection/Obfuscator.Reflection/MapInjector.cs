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
        GetMemberWithParameters,
        GetTypeFromAssembly
    }

    internal static class MapInjector
    {
        private static readonly string GUID;
        static MapInjector()
        {
            GUID = Guid.NewGuid().ToString().ToUpper();
        }

        public static TypeDefinition InjectMap(AssemblyDefinition assembly)
        {
            var module = assembly.MainModule;
            var mapModule = ModuleDefinition.ReadModule(typeof(Map).Module.FullyQualifiedName);
            var mapTypeDefinition = mapModule.GetType(typeof(Map).FullName);

            string destinationName = "<PrivateImplementationDetails>{" + GUID + "}";
            
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
            dict.Clear();
            ILProcessor processor = constructor.Body.GetILProcessor();
            var last = processor.Body.Instructions.Single(i => i.OpCode == OpCodes.Ret);
            foreach (var entry in renameData)
            {
                var member = entry.Key;
                var newName = entry.Value;
                
                Stack<Instruction> procedure = new Stack<Instruction>();
                if (member.DeclaringType != null)
                {
                    string declaringTypeNewName = GetTypeNewName(renameData, member.DeclaringType);

                    if (keepNamespaces)
                        declaringTypeNewName = member.DeclaringType.Namespace + '.' + declaringTypeNewName;

                    if (member.MetadataToken.TokenType == TokenType.Method)
                        procedure = AddMemberWithParameters((MethodReference)member, newName, declaringTypeNewName, addMemberWithParameters, GetParametersString((MethodReference)member, renameData));
                    else if (member.MetadataToken.TokenType == TokenType.Field)
                        procedure = AddMember(member, newName, declaringTypeNewName, addMember);
                }
                else
                    procedure = AddType((TypeDefinition)member, newName, addType, keepNamespaces);

                foreach (var instruction in procedure)
                    processor.InsertBefore(last, instruction);
            }
        }

        private static string GetTypeNewName(IDictionary<IMemberDefinition, string> renameData, TypeDefinition type)
        {
            string typeNewName = GetTypeName(renameData, type);

            if (type.IsNested)
            {
                var declaringType = type.DeclaringType;
                do
                {
                    typeNewName = GetTypeName(renameData,declaringType) + '+' + typeNewName;
                    declaringType = declaringType.DeclaringType;
                } while (declaringType != null);
            }
            return typeNewName;
        }

        private static string GetTypeName(IDictionary<IMemberDefinition, string> renameData, TypeDefinition type)
        {
            string typeNewName;
            if (!renameData.TryGetValue(type, out typeNewName))
                typeNewName = type.FullName;
            return typeNewName;
        }

        static private Dictionary<string, string> dict = new Dictionary<string, string>();
        static Stack<Instruction> AddType(TypeDefinition type, string newName, MethodReference addType, bool keepNamespaces)
        {
            var fullNewName = newName;
            if (keepNamespaces)
                fullNewName = type.Namespace + '.' + fullNewName;
            dict.Add(type.FullName, fullNewName);
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

        static Stack<Instruction> AddMember(IMemberDefinition member, string newName, string declaringTypeNewName,
            MethodReference addMember)
        {
            dict.Add(declaringTypeNewName+"::"+member.Name, newName);
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
            MethodReference addMemberWithParameters, string parametersString)
        {
            dict.Add(declaringTypeNewName + "::" + method.Name + parametersString, newName);
            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addMemberWithParameters));
            result.Push(Instruction.Create(OpCodes.Ldstr, newName));
            result.Push(Instruction.Create(OpCodes.Ldstr, parametersString));
#if HASH
            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMd5Hash(method.Name)));
#else
            result.Push(Instruction.Create(OpCodes.Ldstr, method.Name));
#endif
            result.Push(Instruction.Create(OpCodes.Ldstr, declaringTypeNewName));
            return result;
        }

        static string GetParametersString(MethodReference method, IDictionary<IMemberDefinition, string> renameMap)
        {
            if (method.HasParameters)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append('(');
                foreach (var type in method.Parameters)
                {
                    if(type.ParameterType.IsGenericParameter)
                        builder.Append(type.ParameterType.FullName);
                    else
                        builder.Append(GetTypeNewName(renameMap, type.ParameterType.Resolve()));
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
