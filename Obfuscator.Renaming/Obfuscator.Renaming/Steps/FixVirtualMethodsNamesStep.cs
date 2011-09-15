using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Obfuscator.Utils;

namespace Obfuscator.Steps.Renaming
{
    public class FixVirtualMethodsNames : BaseStep
    {
        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            var renamer = Context.Renamers[assembly.Name];
            foreach (var type in assembly.MainModule.GetAllTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    while (HasAnOriginMethod(method, renamer.DefinitionsMap))
                        renamer.MapDefinition(method);
                    
                }
            }
        }

        // To samo dla metod z interfacow?
        private bool HasAnOriginMethod(MethodDefinition method, IDictionary<IMemberDefinition, string> renameMap)
        {
            if (method.IsVirtual)
            {
                if (!method.IsNewSlot) // is override
                {
                    var baseType = Helper.GetBaseTypeDefinition(method.DeclaringType);
                    while (baseType != null)
                    {
                        if (baseType.Methods.SingleOrDefault(m => m.IsVirtual && IsBaseMethod(m, method, renameMap)) != null)
                        {
                            return true;
                        }
                        baseType = Helper.GetBaseTypeDefinition(baseType);
                    }
                }
                else// is interface implementation, or new virtual method
                {
                    var interfaceMethods = Helper.GetInterfaceMethods(method.DeclaringType);
                    foreach (var interfaceMethod in interfaceMethods)
                    {
                        if(Helper.IsImplicitImplementation(interfaceMethod, method))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool IsBaseMethod(MethodDefinition @base, MethodDefinition implementation, IDictionary<IMemberDefinition, string> renameMap)
        {            
            var baseName = @base.Name;
            var implName = implementation.Name;

            if (renameMap.ContainsKey(@base))
                baseName = renameMap[@base];
            if (renameMap.ContainsKey(implementation))
                implName = renameMap[implementation];

            if (baseName != implName)
                return false;

            if (!Helper.AreSame(@base.ReturnType, implementation.ReturnType))
                return false;

            if (!Helper.AreSame(@base.Parameters, implementation.Parameters))
                return false;

            if (@base.GenericParameters.Count != implementation.GenericParameters.Count)
                return false;

            return true;
        }    
    }
}
