using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Obfuscator.Utils;

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
                    continue;
                // does not need to ad method to overrides since it's already there
                
                var implementation = type.Methods.SingleOrDefault(m => Helper.IsImplicitImplementation(method, m));                
                if (implementation != null)
                {
                    AddOverride(method, implementation);
                    continue;
                }

                //TODO: Create copy of the method above, without the method body, in the method body just call
                // the base method and add interface method to overrides
                //MethodDefinition customMethod = CreateExplicitInterfaceImplementation(type, method);
                //customMethod.
                var @base = Helper.GetBaseMethod(method, type);
                Debug.Assert(@base != null);
            }
        }

        private MethodDefinition CreateExplicitInterfaceImplementation(TypeDefinition declaringType, MethodDefinition method)
        {                        
            var attributes = MethodAttributes.Private | MethodAttributes.Final |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.NewSlot;
            var returnType = declaringType.Module.Import(method.ReturnType);

            MethodDefinition result = new MethodDefinition(method.Name, attributes, returnType);
            declaringType.Methods.Add(result);
            return result;
        }

        private void FindImplementationsOfBaseMethods(TypeDefinition type)
        {
            if (type == null || !type.HasMethods)
                return;

            foreach (var method in type.Methods)
            {                
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
