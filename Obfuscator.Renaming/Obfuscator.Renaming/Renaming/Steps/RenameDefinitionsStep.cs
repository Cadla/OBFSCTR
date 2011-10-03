using System;
using System.Linq;
using Mono.Cecil;
using System.IO;
using System.Collections.Generic;
using Obfuscator.Renaming;

namespace Obfuscator.Renaming.Steps
{
    internal class RenameDefinitionsStep : RenamingBaseStep
    {
        RenamingOptions _options;

        public RenameDefinitionsStep(RenamingOptions options)
        {
            _options = options;
        }

        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            bool keepNamespaces = _options.HasFlag(RenamingOptions.KeepNamespaces);
            var definitionsMap = Context.DefinitionsRenameMap[assembly.Name];

            foreach (var entry in definitionsMap)
            {
                RenameDefinition(entry.Key, entry.Value, keepNamespaces);
            }
            foreach (var resource in assembly.MainModule.Resources)
            {
                RenameResource(resource, definitionsMap, keepNamespaces);
            }
        }

        private static void RenameDefinition(IMemberDefinition member, string newName, bool keepNamespaces)
        {
            if (!keepNamespaces && member.DeclaringType == null)
            {
                ((TypeDefinition)member).Namespace = String.Empty;
            }
            member.Name = newName;
        }

        private static void RenameResource(Resource resource, Dictionary<IMemberDefinition, string> definitionsMap, bool keepNamespaces)
        {
            string name = Path.GetFileNameWithoutExtension(resource.Name);
            IMemberDefinition member;
            if ((member = definitionsMap.Keys.SingleOrDefault(m => m.FullName == name)) != null)
            {
                string suffix = resource.Name.Substring(name.Length);
                var newName = definitionsMap[member];
                newName = newName + suffix;
                if (keepNamespaces && member.DeclaringType == null)
                    newName = ((TypeDefinition)member).Namespace + '.' + newName;

                resource.Name = newName;
            }
        }
    }
}
