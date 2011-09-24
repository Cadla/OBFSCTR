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
    public class RenameReferencesStep : BaseStep
    {
        AssemblyVisitor _visitor = new AssemblyVisitor();
        Dictionary<AssemblyNameReference, List<MemberReference>> _assemblyReferences = new Dictionary<AssemblyNameReference, List<MemberReference>>();
        List<KeyValuePair<MemberReference, string>> toRename = new List<KeyValuePair<MemberReference, string>>();

        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            var resolver = ReferenceResolver.GetDefaultResolver(assembly.MainModule);
            ReferenceVisitor collectReferences = new ReferenceVisitor(resolver, ref _assemblyReferences);
            _visitor.ConductVisit(assembly, collectReferences);
        }

        protected override void EndProcess()
        {
            bool keepNamespaces = Context.Options.HasFlag(ObfuscationOptions.KeepNamespaces);

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
                    if (renameMap.TryGetValue(referenceDefinition, out newName))
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
            //if (reference.DeclaringType == null && !keepNamespaces)
            //        ((TypeReference)reference).Namespace = String.Empty;
            //reference.Name = newName;

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
                if (reference is MethodSpecification)
                    ((MethodSpecification)reference).ElementMethod.Name = newName;
                else
                    reference.Name = newName;
            }
        }
    }
}
