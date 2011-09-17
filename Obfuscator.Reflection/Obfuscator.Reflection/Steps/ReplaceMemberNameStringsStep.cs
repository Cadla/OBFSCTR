using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Steps;
using Obfuscator.MetadataBuilder.Extensions;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;
using Reflection;
using Obfuscator.Utils;

namespace Obfuscator.Steps.Reflection
{
    public class ReplaceMemberNameStringsStep : BaseStep
    {

        protected override void ProcessAssembly(Mono.Cecil.AssemblyDefinition assembly)
        {
            var reference = InjectAndFillMap(assembly);
            foreach (var type in assembly.MainModule.GetAllTypes())
            {
                foreach (var method in type.Methods)
                {
                    if(method.HasBody)
                    {
                        var body = method.Body;
                        var loadInstructions = GetNameLoadInstructions(body);
                        ILProcessor processor = body.GetILProcessor();                        
                        foreach (var instruction in loadInstructions)
                        {                             
                            var callGetNewName = processor.Create(OpCodes.Call, reference);                            
                            processor.InsertAfter(instruction, callGetNewName);
                        }
                    }
                }
            }
        }

        private List<Instruction> GetNameLoadInstructions(MethodBody body)
        {
            IFilter filter = Context.InputConfiguration;
            var loadInstructions = new List<Instruction>();

            foreach (var instruction in body.Instructions)
            {
                var methodReference = instruction.Operand as MethodReference;
                if (methodReference != null)
                {
                    int paramIndex;
                    if (filter.InvokesByName(methodReference, out paramIndex))
                    {
                        var nameLoadInstruction = instruction;
                        var paramCount = methodReference.Parameters.Count - paramIndex + 1;
                        while (paramCount-- != 0)
                        {
                            nameLoadInstruction = nameLoadInstruction.Previous;
                            if (nameLoadInstruction.OpCode == OpCodes.Call)
                                paramCount += ((MethodReference)nameLoadInstruction.Operand).Parameters.Count;
                            else if (nameLoadInstruction.OpCode == OpCodes.Callvirt || nameLoadInstruction.OpCode == OpCodes.Calli)
                                paramCount += ((MethodReference)nameLoadInstruction.Operand).Parameters.Count + 1;
                        }
                        loadInstructions.Add(nameLoadInstruction);
                    }
                }
            }
            return loadInstructions;
        }

        private MethodReference InjectAndFillMap(AssemblyDefinition assembly)
        {
            var module = assembly.MainModule;
            var renameData = Context.DefinitionsRenameMap[assembly.Name].Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value));

            var map = InjectMap(module);
            FillMap(map, renameData);

            //TODO: remove hardcoded string
            MethodDefinition getNewName = map.GetMethods().Single(m => m.Name == "Get");
            var reference = assembly.MainModule.Import(getNewName);
            return reference;
        }

        static TypeDefinition InjectMap(ModuleDefinition module)
        {
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

        static void FillMap(TypeDefinition map, IEnumerable<KeyValuePair<string, string>> renameData)
        {
            var constructor = map.Methods.Single(m => m.IsConstructor);
            var add = map.Methods.Single(m => m.Name == "Add");

            ILProcessor processor = constructor.Body.GetILProcessor();
            var last = processor.Body.Instructions.Single(i => i.OpCode == OpCodes.Ret);
            foreach (var entry in renameData)
            {
                //IL_002a: ldstr "entry.Key"
                //IL_002f: ldstr "entry.Value"
                //IL_0034: call void '<PrivateImplementationDetails>{4DB3961C-4130-42C7-AA8C-5812C5817211}.A'::Add(string, string)
                //IL_0039: nop

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

        
    }
}
