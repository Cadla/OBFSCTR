using System;
using System.Collections.Generic;
using Mono.Cecil;
using Obfuscator.Renaming.Visitors;
using Obfuscator.Utils;
using Obfuscator.Renaming;

namespace Obfuscator.Renaming.Steps
{
    internal class RenameReferencesStep : RenamingBaseStep
    {
        AssemblyVisitor _visitor = new AssemblyVisitor();
        RenamingOptions _options;

        Dictionary<AssemblyNameReference, List<MemberReference>> _assemblyReferences = new Dictionary<AssemblyNameReference, List<MemberReference>>();
        List<KeyValuePair<MemberReference, string>> toRename = new List<KeyValuePair<MemberReference, string>>();
              
        public RenameReferencesStep(RenamingOptions options)
        {
            _options = options;
        }

        protected override void ProcessAssembly(AssemblyDefinition assembly)
        { 
            ReferencesMapVisitor collectReferences = new ReferencesMapVisitor(ref _assemblyReferences);
            _visitor.ConductVisit(assembly, collectReferences);
        }

        protected override void EndProcess()
        {
            bool keepNamespaces = _options.HasFlag(RenamingOptions.KeepNamespaces);

            foreach (var assembly in _assemblyReferences)
            {
                Dictionary<IMemberDefinition, string> renameMap = GetRenameMap(assembly.Key);
                if (renameMap == null)
                    continue;

                foreach (var reference in assembly.Value)
                {
                    //TODO: !!! zmienić !!!
                    IMemberDefinition referenceDefinition = ((dynamic)reference).Resolve();
                    string newName;
                    if (referenceDefinition != null && renameMap.TryGetValue(referenceDefinition, out newName))
                        toRename.Add(new KeyValuePair<MemberReference, string>(reference, newName));                        
                }
            }

            foreach (var reference in toRename)
                RenameReference(reference.Key, reference.Value, keepNamespaces);
        }

        private Dictionary<IMemberDefinition, string> GetRenameMap(AssemblyNameReference assembly)
        {            
            foreach (var map in Context.DefinitionsRenameMap)
                if (map.Key.FullName == assembly.FullName)
                    return map.Value;
            return null;
        }

        private void RenameReference(MemberReference reference, string newName, bool keepNamespaces)
        {
            var typeReference = reference as TypeReference;
            if (typeReference != null)
            {
                typeReference = typeReference.GetElementType();
                if (!typeReference.IsNested && !keepNamespaces)
                    typeReference.Namespace = String.Empty;
                typeReference.Name = newName;
            }
            else
            {
                //TODO: renamin of method specifications not needed?
                if (reference is MethodSpecification)
                    ((MethodSpecification)reference).ElementMethod.Name = newName;
                else
                    reference.Name = newName;
            }
        }
    }
}
