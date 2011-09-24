using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;
using System.Diagnostics;
using Obfuscator.Renaming;

namespace Obfuscator.Steps.Renaming
{
    public class RenameDefinitionsStep : BaseStep
    {
        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            foreach (var entry in Context.DefinitionsRenameMap[assembly.Name])
            {                 
               RenameDefinition(entry.Key, entry.Value, Context.Options.HasFlag(ObfuscationOptions.KeepNamespaces));
            }
            foreach (var entry in Context.ResourcesRenameMap[assembly.Name])
            {
                RenameResource(entry.Key, entry.Value);
            }            
        }

        private void RenameDefinition(IMemberDefinition member, string newName, bool keepNamespaces)
        {
            if (!Context.Options.HasFlag(ObfuscationOptions.KeepNamespaces) && member.DeclaringType == null)            
            {                
                ((TypeDefinition)member).Namespace = String.Empty;                
            }
            member.Name = newName;
        }

        private void RenameResource(Resource resource, string newName)
        {
            resource.Name = newName;
        }

    }
}
