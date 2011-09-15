using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
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
                    implementation.Overrides.Add(method);
                }
            }
        }

        private void FindImplementationsOfInterfaceMethods(TypeDefinition type)
        {
            foreach (var method in Helper.GetInterfaceMethods(type))
            {
                bool test = false;
                var currentType = type;
                do
                {
                    MethodDefinition implementation = currentType.Methods.SingleOrDefault(m => Helper.IsExplicitImplementation(method, m));
                    // does not need to ad method to overrides since it's already there
                    if (implementation != null)
                    {
                        test = true;
                        break;
                    }
                    implementation = currentType.Methods.SingleOrDefault(m => Helper.IsImplicitImplementation(method, m));
                    if (implementation != null)
                    {
                        AddOverride(method, implementation);
                        test = true;
                        break;
                    }
                    currentType = Helper.GetBaseTypeDefinition(currentType);
                } while (currentType != null);
                Debug.Assert(test);
            }
        }

        private void FindImplementationsOfBaseMethods(TypeDefinition type)
        {
            if (type == null || !type.HasMethods)
                return;

            foreach (var method in type.Methods)
            {
                if (method.IsVirtual && !method.IsNewSlot)
                {
                    var baseType = Helper.GetBaseTypeDefinition(type);            
                    while (baseType != null)
                    {
                        var origin = baseType.Methods.SingleOrDefault(m => Helper.IsOverrideCompliant(m, method));
                        if (origin != null)
                        {
                            AddOverride(origin, method);
                            break;
                        }
                        baseType = Helper.GetBaseTypeDefinition(baseType);
                    }
                }
               
            }
        }

        private void AddOverride(MethodDefinition origin, MethodDefinition implementation)
        {
            MethodReference originReference = origin;
            if (origin.Module != implementation.Module)
                originReference = implementation.Module.Import(origin);

            if (_methodOverrides.ContainsKey(implementation))
                _methodOverrides[implementation].Add(originReference);
            else
                _methodOverrides.Add(implementation, new List<MethodReference>() { originReference });
        }
    }
}
