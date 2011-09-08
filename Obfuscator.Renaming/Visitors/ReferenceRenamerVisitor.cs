using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Utils;
using Mono.Cecil;

namespace Obfuscator.Renaming.Visitors
{
    public class ReferenceRenamerVisitor : NullAssemblyVisitor
    {
        private Renamer _renamer;
        IDictionary<AssemblyNameDefinition, HashSet<MemberReference>> _visited;

        public IDictionary<AssemblyNameDefinition, HashSet<MemberReference>> RenameMap
        {
            get
            {
                return _visited;
            }
        }

        public IDictionary<MemberReference, IMemberDefinition> ResolvedMembers
        {
            get;
            private set;
        }

        public override VisitorLevel Level()
        {
            return VisitorLevel.MethodBodys;
        }

        public ReferenceRenamerVisitor(Renamer renamer)
        {
            _renamer = renamer;
            _visited = new Dictionary<AssemblyNameDefinition, HashSet<MemberReference>>();
            ResolvedMembers = new Dictionary<MemberReference, IMemberDefinition>(); 
        }

        public override void VisitEventReference(Mono.Cecil.EventReference @event)
        {
            IMemberDefinition definition = @event.Resolve();
            var assemblyName = GetAssemblyName(definition);

            if (Visited(@event, assemblyName))
                return;

            ResolvedMembers[@event] = definition;
            Visit(@event, assemblyName);
        }


        public override void VisitFieldReference(Mono.Cecil.FieldReference field)
        {
            IMemberDefinition definition = field.Resolve();
            var assemblyName = GetAssemblyName(definition);

            if (Visited(field, assemblyName))
                return;

            ResolvedMembers[field] = definition;
            Visit(field, assemblyName);
        }

        public override void VisitMethodReference(Mono.Cecil.MethodReference method)
        {
            IMemberDefinition definition = method.Resolve();
            var assemblyName = GetAssemblyName(definition);

            if (Visited(method, assemblyName))
                return;

            ResolvedMembers[method] = definition;
            Visit(method, assemblyName);
        }

        public override void VisitPropertyReference(Mono.Cecil.PropertyReference property)
        {
            IMemberDefinition definition = property.Resolve();
            var assemblyName = GetAssemblyName(definition);

            if (Visited(property, assemblyName))
                return;

            ResolvedMembers[property] = definition;
            Visit(property, assemblyName);
        }

        public override void VisitTypeReference(Mono.Cecil.TypeReference type)
        {
            IMemberDefinition definition = type.Resolve();
            var assemblyName = GetAssemblyName(definition);

            if (Visited(type, assemblyName))
                return;

            ResolvedMembers[type] = definition;
            Visit(type, assemblyName);
        }

        private AssemblyNameDefinition GetAssemblyName(IMemberDefinition definition)
        {
            if (definition.DeclaringType == null && definition is TypeDefinition)
            {
                var typeDefinition = definition as TypeDefinition;
                return typeDefinition.Module.Assembly.Name;
            }
            else
            {
                return definition.DeclaringType.Module.Assembly.Name;
            }
        }

        private bool Visited(MemberReference member, AssemblyNameDefinition assemblyName)
        {
            if (Helper.IsCoreAssemblyName(assemblyName.Name))
                return true;
            if (!_visited.ContainsKey(assemblyName))
                return false;
            return _visited[assemblyName].Contains(member);
        }

        private void Visit(MemberReference member, AssemblyNameDefinition assemblyName)
        {            
            if (!_visited.ContainsKey(assemblyName))
            {
                _visited.Add(assemblyName, new HashSet<MemberReference>() { member });
            }
            else
            {
                var assembly = _visited[assemblyName];
                if (!assembly.Contains(member))
                {
                    assembly.Add(member);
                }
            }
        }
    }
}
