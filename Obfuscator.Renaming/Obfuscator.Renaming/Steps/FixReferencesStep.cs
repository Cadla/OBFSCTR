using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;
using Obfuscator.Steps;

namespace Obfuscator.Steps.Renaming
{
    public class FixReferencesStep : BaseStep
    {            
        protected override void ProcessAssembly(Mono.Cecil.AssemblyDefinition assembly)
        {
            var references = Context.ReferencesRenameMap[assembly.Name].ToList();
            foreach (var reference in references)
            {
                dynamic dynamicReference = reference.Key;
                IMemberDefinition definition = dynamicReference.Resolve();
                var assemblyName = Helper.GetAssemblyName(definition);

                if (Context.DefinitionsRenameMap.ContainsKey(assemblyName)) // renaming was done in this assembly
                {
                    string newName;
                    if (Context.DefinitionsRenameMap[assemblyName].TryGetValue(definition, out newName))
                    {
                        Context.ReferencesRenameMap[assemblyName][reference.Key] = newName;
                    }
                }
            }
        }

        //protected override void EndProcess()
        //{
        //    foreach (var assembly in Context.RanamingContext)
        //    {
        //        assembly.Value.RenameAll();
        //    }

        //    foreach (var reference in _memberReferences)
        //    {                
        //        reference.Key.Name = reference.Value;
        //    }
        //    foreach (var reference in _typeReferences)
        //    {
        //        reference.Key.Name = reference.Value.Value;
        //        reference.Key.Namespace = reference.Value.Key;
        //    }        
        //}
    }
}

