using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;
using Obfuscator.Renaming.Visitors;

namespace Obfuscator.Renaming.Steps
{
    public class FixReferencesStep : BaseStep
    {
        AssemblyVisitor _visitor;
        IDictionary<AssemblyNameDefinition, HashSet<MemberReference>> _references;
        IDictionary<MemberReference, IMemberDefinition> _resolved;

        public FixReferencesStep()
        {
            _visitor = new AssemblyVisitor();
            _references = new Dictionary<AssemblyNameDefinition, HashSet<MemberReference>>();
            _resolved = new Dictionary<MemberReference, IMemberDefinition>();
        }

        protected override void ProcessAssembly(Mono.Cecil.AssemblyDefinition assembly)
        {
            var renamer = Context.Renamers[assembly.Name];
            ReferenceRenamerVisitor referanceRenamer = new ReferenceRenamerVisitor(renamer);
            _visitor.ConductVisit(assembly, referanceRenamer);
            AddReferences(referanceRenamer.RenameMap);
            AddResolved(referanceRenamer.ResolvedMembers);
        }

        protected override void EndProcess()
        {
            foreach (var assembly in _references)
            {
                var assemblyDefinition = Context.Resolve(assembly.Key);
                var resolver = assemblyDefinition.MainModule.MetadataResolver;
                if (Context.Renamers.ContainsKey(assembly.Key))
                {
                    var renamer = Context.Renamers[assembly.Key];
                    foreach (var member in assembly.Value)
                    {
                        var memberDefinition = _resolved[member];
                        if (renamer.RenameMap.ContainsKey(memberDefinition))
                        {
                            member.Name = renamer.RenameMap[memberDefinition];
                            var typeReference = member as TypeReference;
                            if (typeReference != null)
                                typeReference.Namespace = "";
                        }
                    }
                }
            }
        }

        private void AddReferences(IDictionary<AssemblyNameDefinition, HashSet<MemberReference>> newSet){
            foreach (var assembly in newSet)
            {
                if (_references.ContainsKey(assembly.Key))
                {
                    _references[assembly.Key].UnionWith(assembly.Value);
                }
                else
                {
                    _references.Add(assembly.Key, assembly.Value);
                }
            }
        }

        private void AddResolved(IDictionary<MemberReference, IMemberDefinition> newSet)
        {
            foreach (var member in newSet)
                if (!_resolved.ContainsKey(member.Key))
                    _resolved.Add(member);
        }
    }
}
