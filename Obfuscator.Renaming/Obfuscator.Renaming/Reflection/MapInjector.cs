using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Obfuscator.MetadataBuilder;
using Obfuscator.Utils;

namespace Obfuscator.Renaming.Reflection
{
    internal static class MapInjector
    {
        // Every map in each assembly has to have the same name, so it can be reference by GetTypeNameFromAssembly method.
        private static readonly string GUID;
        private const string MAP_NAME_PREFIX = "<PrivateImplementationDetails>";

        static MapInjector()
        {
            GUID = Guid.NewGuid().ToString().ToUpper();
        }

        public static TypeDefinition InjectMap(AssemblyDefinition assembly)
        {
            var module = assembly.MainModule;
            var mapModule = ModuleDefinition.ReadModule(typeof(Map).Module.FullyQualifiedName);
            var mapTypeDefinition = mapModule.GetType(typeof(Map).FullName);

            //string destinationName = String.Concat(MAP_NAME_PREFIX, '{', GUID, '}');
            string destinationName = "Map";

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

        public static void FillMap(TypeDefinition map, IDictionary<IMemberDefinition, string> renameData, ReflectionOptions options, bool keepNamespaces)
        {
            var constructor = map.Methods.Single(m => m.IsConstructor);

            //TODO: refactor - hardcodes strings
            var addTypeName = map.Methods.Single(m => m.Name == "AddTypeName");            
            var addFieldName = map.Methods.Single(m => m.Name == "AddFieldName");
            var addEventName = map.Methods.Single(m => m.Name == "AddEventName");
            var addMethodName = map.Methods.Single(m => m.Name == "AddMethodName");
            var addPropertyName = map.Methods.Single(m => m.Name == "AddPropertyName");
            var addNestedTypeName = map.Methods.Single(m => m.Name == "AddNestedTypeName");

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
                    var declaringTypeNewName = GetTypeNewName(renameData, member.DeclaringType, keepNamespaces);
                    switch (member.MetadataToken.TokenType)
                    {
                        case TokenType.Method:
                            if (!options.HasFlag(ReflectionOptions.Methods))
                                continue;
                            var method = (MethodDefinition)member;
                            procedure = AddMemberWithParameters(method, newName, declaringTypeNewName,
                                GetParametersString(method.Parameters, renameData, keepNamespaces), addMethodName);
                            break;
                        case TokenType.Property:
                            if (!options.HasFlag(ReflectionOptions.Properties))
                                continue;
                            var property = (PropertyDefinition)member;
                            procedure = AddMemberWithParameters(property, newName, declaringTypeNewName,
                                GetParametersString(property.Parameters, renameData, keepNamespaces), addPropertyName);
                            break;
                        case TokenType.Field:
                            if (!options.HasFlag(ReflectionOptions.Fields))
                                continue;
                            var field = (FieldDefinition)member;
                            procedure = AddMember(field, newName, declaringTypeNewName, addFieldName);
                            break;
                        case TokenType.Event:
                            if (!options.HasFlag(ReflectionOptions.Events))
                                continue;
                            var @event = (EventDefinition)member;
                            procedure = AddMember(@event, newName, declaringTypeNewName, addEventName);
                            break;
                        case TokenType.TypeDef:
                            if (!options.HasFlag(ReflectionOptions.NestedTypes))
                                continue;
                            var type = (TypeDefinition)member;
                            procedure = AddMember(type, newName, declaringTypeNewName, addNestedTypeName);
                            break;
                    }
                }
                else if (options.HasFlag(ReflectionOptions.Types))                    
                    procedure = AddTypeName((TypeDefinition)member, newName, addTypeName, keepNamespaces);

                foreach (var instruction in procedure)
                    processor.InsertBefore(last, instruction);
            }
        }

        //TODO: fix the type name so it in Type::FullyQualifiedName format
        private static string GetTypeNewName(IDictionary<IMemberDefinition, string> renameData, TypeDefinition type, bool keepNamespaces)
        {
            string typeNewName = GetTypeNewName(renameData, type);

            if (type.IsNested)
            {
                var declaringType = type.DeclaringType;
                do
                {
                    typeNewName = GetTypeNewName(renameData, declaringType) + '+' + typeNewName;
                    declaringType = declaringType.DeclaringType;
                } while (declaringType != null);
            }
            if(keepNamespaces)
                typeNewName = type.Namespace + '.' + typeNewName;
            return typeNewName;
        }

        private static string GetTypeNewName(IDictionary<IMemberDefinition, string> renameData, TypeDefinition type)
        {
            string typeNewName;
            if (!renameData.TryGetValue(type, out typeNewName))
                typeNewName = type.FullName;
            return typeNewName;
        }

        static private Dictionary<string, string> dict = new Dictionary<string, string>();
        static Stack<Instruction> AddTypeName(TypeDefinition type, string newName, MethodReference addType, bool keepNamespaces)
        {
            var fullNewName = newName;
            if (keepNamespaces)
                fullNewName = type.Namespace + '.' + fullNewName;

            dict.Add(type.FullName, fullNewName);

            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addType));
            result.Push(Instruction.Create(OpCodes.Ldstr, fullNewName));
            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMemberName(type.FullName)));
            return result;
        }

        static Stack<Instruction> AddMember(IMemberDefinition member, string newName, string declaringTypeNewName,
            MethodReference addMember)
        {
            if (member is FieldDefinition)
                dict.Add(Map.GetFieldKey(declaringTypeNewName, member.Name), newName);
            else if(member is EventDefinition)
                dict.Add(Map.GetEventKey(declaringTypeNewName, member.Name), newName);
            else
                dict.Add(Map.GetNestedTypeKey(declaringTypeNewName, member.Name), newName);

            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addMember));
            result.Push(Instruction.Create(OpCodes.Ldstr, newName));
            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMemberName(member.Name)));
            result.Push(Instruction.Create(OpCodes.Ldstr, declaringTypeNewName));
            return result;
        }


        static Stack<Instruction> AddMemberWithParameters(IMemberDefinition member, string newName, string declaringTypeNewName, string parametersString, MethodReference addMemberWithParameters)
        {
            if (member is MethodDefinition)
                dict.Add(Map.GetMethodKey(declaringTypeNewName, member.Name, parametersString), newName);
            else
                dict.Add(Map.GetPropertyKey(declaringTypeNewName, member.Name, parametersString), newName);

            var result = new Stack<Instruction>();
            result.Push(Instruction.Create(OpCodes.Nop));
            result.Push(Instruction.Create(OpCodes.Call, addMemberWithParameters));
            result.Push(Instruction.Create(OpCodes.Ldstr, newName));
            result.Push(Instruction.Create(OpCodes.Ldstr, parametersString));
            result.Push(Instruction.Create(OpCodes.Ldstr, Map.GetMemberName(member.Name)));
            result.Push(Instruction.Create(OpCodes.Ldstr, declaringTypeNewName));
            return result;
        }

        static string GetParametersString(IList<ParameterDefinition> parameters, IDictionary<IMemberDefinition, string> renameMap, bool keepNamespaces)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            if (parameters.Count > 0)
            {
                foreach (var type in parameters)
                {
                    if (type.ParameterType.IsGenericParameter)
                        builder.Append(type.ParameterType.FullName);
                    else
                        builder.Append(GetTypeNewName(renameMap, type.ParameterType.Resolve(), keepNamespaces));
                    builder.Append(',');
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append(')');
            return builder.ToString();
        }
    }
}
//Note: Microsoft has defined a Backus-Naur Form grammar for type names and assemblyqualified type names that is used for constructing strings that will be passed to reflection methods. Knowledge of the grammar can come in quite handy when you are using reflection, specifically if you are working with nested types, generic types, generic methods, reference parameters, or arrays. For the complete grammar, see the FCL documentation or do a Web search for “Backus-Naur Form Grammar for Type Names.” You can also look at Type’s MakeArrayType, MakeByRefType, MakeGenericType, and MakePointerType methods.