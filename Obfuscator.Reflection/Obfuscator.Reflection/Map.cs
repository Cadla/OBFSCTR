using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Mono.Cecil;
using Obfuscator.Utils;
using Mono.Cecil.Cil;

namespace Reflection
{
    internal static class MapInjector
    {        
        static TypeDefinition mapTypeDefinition;

        static MapInjector()
        {
            var module = ModuleDefinition.ReadModule(typeof(Map).Module.FullyQualifiedName);
            mapTypeDefinition = module.GetType(typeof(Map).FullName);
        }
        
        internal static void FillMap(TypeDefinition map, IDictionary<string, string> renameData)
        {
            var constructor = map.Methods.Single(m => m.IsConstructor);
            var add = map.Methods.Single(m => m.Name == "Add");
            //IL_002a: ldstr "Method"
            //IL_002f: ldstr "RenamedMethod"
            //IL_0034: call void '<PrivateImplementationDetails>{4DB3961C-4130-42C7-AA8C-5812C5817211}.A'::Add(string, string)
            //IL_0039: nop

            ILProcessor processor = constructor.Body.GetILProcessor();
            var last = processor.Body.Instructions.Single(i => i.OpCode == OpCodes.Ret);
            foreach (var entry in renameData)
            {
                Instruction ldOldName = Instruction.Create(OpCodes.Ldstr, entry.Key);
                Instruction ldNewName = Instruction.Create(OpCodes.Ldstr, entry.Value);
                Instruction callAdd = Instruction.Create(OpCodes.Call, add);
                Instruction nop = Instruction.Create(OpCodes.Nop);
                processor.InsertBefore(last, ldOldName);
                processor.InsertBefore(last, ldNewName);
                processor.InsertBefore(last, callAdd);
                processor.InsertBefore(last, nop);
            }

        }

        internal static TypeDefinition InjectMap(ModuleDefinition module)
        {            
            string destinationName = "<PrivateImplementationDetails>{" + Guid.NewGuid().ToString().ToUpper() + "}";

            TypeDefinition newType = new TypeDefinition(destinationName, "A" , mapTypeDefinition.Attributes)
            {
                PackingSize = mapTypeDefinition.PackingSize,
                ClassSize = mapTypeDefinition.ClassSize,
            };

            newType.BaseType = module.Import(typeof(object));
            module.Types.Add(newType);

            foreach (var field in mapTypeDefinition.Fields)
            {
                AddField(module, newType, field);
            }

            foreach (var method in mapTypeDefinition.Methods)
            {
                AddMethod(module, newType, method);
            }

            foreach (var method in mapTypeDefinition.Methods)
            {
                CopyInstructions(method, newType.Methods.First(m => m.Name == method.Name));   
            }

            return newType;
        }

        private static void AddMethod(ModuleDefinition module, TypeDefinition targetType, MethodDefinition method)
        {
            var newMethod = new MethodDefinition(method.Name, method.Attributes, GetTypeReference(module, method.ReturnType))
            {
                ExplicitThis = method.ExplicitThis,
                ImplAttributes = method.ImplAttributes,
                SemanticsAttributes = method.SemanticsAttributes,
                DeclaringType = targetType,
                CallingConvention = method.CallingConvention,
            };

            targetType.Methods.Add(newMethod);
            
            foreach (var parameterDefinition in method.Parameters)
            {
                var parameterType = GetTypeReference(module, parameterDefinition.ParameterType);

                ParameterDefinition newParameter = new ParameterDefinition(parameterDefinition.Name, parameterDefinition.Attributes, parameterType)
                {
                    HasConstant = parameterDefinition.HasConstant,
                    MarshalInfo = parameterDefinition.MarshalInfo,
                };

                newMethod.Parameters.Add(parameterDefinition);
            }

            newMethod.Body.InitLocals = method.Body.InitLocals;

            foreach (var variableDefinition in method.Body.Variables)
            {
                var variableType = GetTypeReference(module, variableDefinition.VariableType);
                VariableDefinition newVariable = new VariableDefinition(variableDefinition.Name, variableType);
                newMethod.Body.Variables.Add(newVariable);
            }         
        }

        private static void CopyInstructions(MethodDefinition sourceMethod, MethodDefinition targetMethod)
        {
            TypeDefinition targetType = targetMethod.DeclaringType;

            var processor = targetMethod.Body.GetILProcessor();
            var offset = 0;
            foreach (var instruction in sourceMethod.Body.Instructions)
            {
                object operand;

                if (instruction.Operand is FieldReference)
                {
                    operand = GetFieldReference(targetType, (FieldReference)instruction.Operand);
                }
                else if (instruction.Operand is MethodReference)
                {
                    operand = GetMethodReference(targetType, (MethodReference)instruction.Operand);                        
                }
                else if (instruction.Operand is TypeReference)
                {
                    operand = GetTypeReference(targetType.Module, (TypeReference)instruction.Operand);
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

        private static Instruction GetInstruction(Mono.Cecil.Cil.MethodBody methodBody, Instruction instruction)
        {
            if (instruction == null)
                return null;

            return methodBody.Instructions.First(x => x.Offset == instruction.Offset);
        }

        private static void AddField(ModuleDefinition module, TypeDefinition targetType, FieldDefinition field)
        {
            TypeReference fieldType = GetTypeReference(module, field.FieldType);

            FieldDefinition newField = new FieldDefinition(field.Name, field.Attributes, fieldType)
            {
                DeclaringType = targetType,
            };

            targetType.Fields.Add(newField);
        }

        private static TypeReference GetTypeReference(ModuleDefinition module, TypeReference type)
        {
            TypeReference fieldType;
            if (!module.GetTypeReferences().Any(t => Helper.AreSame(t, type)))
            {
                fieldType = module.Import(type); //TODO: shouldn't it import from the type's definition
            }
            else
            {
                fieldType = module.GetTypeReferences().First(t => Helper.AreSame(t, type));
            }
            return fieldType;
        }

        private static MethodReference GetMethodReference(TypeDefinition type, MethodReference method)
        {
            MethodReference methodReference;
            if (!Helper.IsCore(method.DeclaringType) && type.Methods.Any(m => AreSame(m, method)))
            {
                return type.Methods.Single(m => AreSame(m, method));
            }
            //if (!type.Module.GetMemberReferences().Any(m => m is MethodReference && AreSame((MethodReference)m, method)))
            //{
                methodReference = type.Module.Import(method); //TODO: shouldn't it import from the type's definition
            //}
            //else
            //{
            //    methodReference = (MethodReference)type.Module.GetMemberReferences().First(m => m is MethodReference && AreSame((MethodReference)m, method));
            //}
            return methodReference;
        }


        private static FieldReference GetFieldReference(TypeDefinition type, FieldReference field)
        {
            FieldReference fieldReference;
            if (!Helper.IsCore(field.DeclaringType) && type.Fields.Any(f => Helper.AreSame(f, field)))
            {
                return type.Fields.Single(f => Helper.AreSame(f, field));
            }
            //if (!type.Module.GetMemberReferences().Any(f => f is FieldReference && Helper.AreSame((FieldReference)f, field)))
            //{
                fieldReference = type.Module.Import(field); //TODO: shouldn't it import from the type's definition
            //}
            //else
            //{
            //    fieldReference = (FieldReference)type.Module.GetMemberReferences().First(f => f is FieldReference && Helper.AreSame((FieldReference)f, field));
            //}
            return fieldReference;
        }
        
        public static bool AreSame(MethodReference a, MethodReference b)
        {
            if (a.Name != b.Name)
                return false;

            if (!Helper.AreSame(a.ReturnType, b.ReturnType))
                return false;

            if (!Helper.AreSame(a.Parameters, b.Parameters))
                return false;

            if (a.GenericParameters.Count != b.GenericParameters.Count)
                return false;

            return true;
        }
    }

    internal static class Map
    {
        private static MD5 hash;
        private static Dictionary<string, string> NameMap;

        private static void Add(string oldName, string newName)
        {
            NameMap.Add(GetMd5Hash(oldName), newName);
        }

        static Map()
        {
            hash = MD5.Create();
            NameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            //Add("", "");
            //Add("Method", "RenamedMethod");
            //Add("Reflection.Class", "Reflection.RenamedClass");            
        }

        static string GetMd5Hash(string input)
        {
            byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }     

        public static string Get(string str)        
        {
            string result;
            string hashed = GetMd5Hash(str);
            if (NameMap.TryGetValue(hashed, out result))
                return result;
            return str;
        }

   
    }
}

