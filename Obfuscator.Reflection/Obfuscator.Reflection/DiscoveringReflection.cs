using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Utils;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Reflection
{
    public class DiscoveringReflection : NullAssemblyVisitor
    {
        List<MethodReference> _reflectionMethods;
        TypeDefinition map;

        public DiscoveringReflection()
        {
            ModuleDefinition corlib = ModuleDefinition.ReadModule(typeof(object).Module.FullyQualifiedName);
            var typeType = corlib.GetType("System.Type");
            var methods = typeType.GetMethods().Where(m => m.HasParameters && m.Parameters[0].ParameterType.FullName == "System.String");
            var assemblyType = corlib.GetType("System.Reflection.Assembly");
            var assemblyMethods = assemblyType.GetMethods().Where(m => m.HasParameters && m.Parameters[0].ParameterType.FullName == "System.String");

            _reflectionMethods = methods.Union(assemblyMethods).Cast<MethodReference>().ToList();
        }

        public override VisitorLevel Level()
        {
            return VisitorLevel.MethodBodys;
        }

        public override void VisitModuleDefinition(ModuleDefinition module)
        {
            map = MapInjector.InjectMap(module);
            MapInjector.FillMap(map, new Dictionary<string, string>() {
            {"Reflection.Class", "Reflection.RenamedClass"}, {"Method", "RenamedMethod"}});
        }

        public override void VisitMethodBody(MethodBody body)
        {
            ILProcessor processor = body.GetILProcessor();
            MethodDefinition getNewName = map.GetMethods().Single(m => m.Name == "Get");

            var reference = body.Method.Module.Import(getNewName);

            var instructions = new List<Instruction>();

            foreach (var instruction in body.Instructions)
            {
                object operand = instruction.Operand;

                if (operand is MethodReference)
                {
                    var method = operand as MethodReference;
                    if (_reflectionMethods.Any(m => Helper.AreSame(m, method)))
                    {
                        var previous = instruction.Previous;
                        if (previous.OpCode.Code == Code.Ldstr)
                        {
                            //      Console.WriteLine(previous.Operand as string);
                            //      processor.Remove(previous);

                            instructions.Add(instruction);
                        }
                    }
                }
            }

            foreach (var instruction in instructions)
            {
                Instruction callGetNewName = processor.Create(OpCodes.Call, reference);
                processor.InsertBefore(instruction, callGetNewName);
            }
        }



    }
}
