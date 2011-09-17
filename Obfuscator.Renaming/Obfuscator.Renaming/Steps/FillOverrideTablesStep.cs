using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Obfuscator.Utils;
using Obfuscator.MetadataBuilder.Extensions;
using Mono.Cecil.Cil;

namespace Obfuscator.Steps.Renaming
{
    public class FillOverrideTables : BaseStep
    {
        Dictionary<MethodDefinition, List<MethodReference>> _methodOverrides;

        public FillOverrideTables()
        {
            _methodOverrides = new Dictionary<MethodDefinition, List<MethodReference>>();
        }

        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            base.ProcessAssembly(assembly);

            foreach (TypeDefinition type in assembly.MainModule.GetTypes().Where(t => !t.IsInterface))
            {
                FindImplementationsOfBaseMethods(type);
                FindImplementationsOfInterfaceMethods(type);
            }
        }

        protected override void EndProcess()
        {
            foreach (var key in _methodOverrides)
            {
                var implementation = key.Key;
                foreach (var method in key.Value)
                {
                    var reference = method.Module.Import(method);
                    if(!implementation.Overrides.Contains(reference))
                        implementation.Overrides.Add(reference);
                }
            }
        }

        private void FindImplementationsOfInterfaceMethods(TypeDefinition type)
        {
            foreach (var method in Helper.GetInterfaceMethods(type))
            {                
                if(type.Methods.Any(m => Helper.IsExplicitImplementation(method, m)))
                    // does not need to ad method to overrides since it's already there
                    continue;
                
                
                var implementation = type.Methods.SingleOrDefault(m => Helper.IsImplicitImplementation(method, m));                
                if (implementation != null)
                {
                    AddOverride(method, implementation);
                    continue;
                }

                var customMethod = CreateExplicitInterfaceImplementation(type, method);
                AddOverride(method, customMethod);                
            }
        }

        private static MethodDefinition CreateExplicitInterfaceImplementation(TypeDefinition type, MethodDefinition method)
        {
            var customMethod = type.InjectMethod(method, ReferenceResolver.GetDefaultResolver(type.Module), false);
            
            customMethod.Attributes = MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot;          
                       

            var body = customMethod.Body = new MethodBody(customMethod);
            body.InitLocals = true;
            var returnType = type.Module.Import(method.ReturnType);
            body.Variables.Add(new VariableDefinition(returnType));
            var processor = customMethod.Body.GetILProcessor();
            processor.Emit(OpCodes.Nop);
                        
            for (byte i = 0; i < method.Parameters.Count + 1; ++i)
            {
                switch (i)
                {
                    case 0:
                        processor.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        processor.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        processor.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        processor.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        processor.Emit(OpCodes.Ldarg_S, i);
                        break;

                }
            }

            var @base = Helper.GetBaseMethod(method, type);
            
            processor.Emit(OpCodes.Call, @base);
            processor.Emit(OpCodes.Stloc_0);
            var ldloc = processor.Create(OpCodes.Ldloc_0);
            processor.Append(ldloc);
            var brs = processor.Create(OpCodes.Br_S, ldloc);
            processor.InsertBefore(ldloc, brs);
            processor.Emit(OpCodes.Ret);

            return customMethod;
        }

        private void FindImplementationsOfBaseMethods(TypeDefinition type)
        {
            if (type == null || !type.HasMethods)
                return;

            foreach (var method in type.Methods)
            {
                if (!method.IsVirtual || method.IsNewSlot)
                    continue;

                var @base = Helper.GetBaseMethod(method);
                if(@base != method)
                    AddOverride(@base, method);               
            }
        }

        private void AddOverride(MethodDefinition @base, MethodDefinition implementation)
        {
            MethodReference baseReference = @base;
            if (@base.Module != implementation.Module)
                baseReference = implementation.Module.Import(@base);

            if (_methodOverrides.ContainsKey(implementation))
                _methodOverrides[implementation].Add(baseReference);
            else
                _methodOverrides.Add(implementation, new List<MethodReference>() { baseReference });
        }
    }
}
