using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;
using System.Diagnostics;

namespace Obfuscator.Steps.Renaming
{
    public class RenameStep : BaseStep
    {
        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            foreach (var entry in Context.DefinitionsRenameMap[assembly.Name])
            {                 
               RenameDefinition(entry.Key, entry.Value);
            }
            foreach (var entry in Context.ResourcesRenameMap[assembly.Name])
            {
                RenameResource(entry.Key, entry.Value);
            }
            foreach (var entry in Context.ReferencesRenameMap[assembly.Name])
            {
                RenameReference(entry.Key, entry.Value);
            }
        }

        private void RenameDefinition(IMemberDefinition member, string newName)
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

        private void RenameReference(MemberReference reference, string newName)
        {
            if (newName == String.Empty)
                return;
            var typeReference = reference as TypeReference;
            if (typeReference != null)
            {
                typeReference = typeReference.GetElementType();
                if (!typeReference.IsNested && !Context.Options.HasFlag(ObfuscationOptions.KeepNamespaces))
                    typeReference.Namespace = String.Empty;
                typeReference.Name = newName;
            }
            else
            {
                if (reference is MethodSpecification)
                    ((MethodSpecification)reference).ElementMethod.Name = newName;
                else
                    reference.Name = newName;
            }
        }
    }
}
