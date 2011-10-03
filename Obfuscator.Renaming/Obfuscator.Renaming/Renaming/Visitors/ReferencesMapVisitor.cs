using System.Collections.Generic;
using Mono.Cecil;
using Obfuscator.Utils;

namespace Obfuscator.Renaming.Visitors
{
    public class ReferencesMapVisitor : NullAssemblyVisitor
    {
        private Dictionary<AssemblyNameReference, List<MemberReference>> _references;

        public ReferencesMapVisitor(ref Dictionary<AssemblyNameReference, List<MemberReference>> references)
        {
            _references = references;
        }

        public override void VisitEventReference(EventReference @event)
        {
            var eventReference = @event;
            var scope = GetAssemblyNameReference(eventReference.DeclaringType.Scope);
            MapReference(scope, eventReference);
        }

        public override void VisitFieldReference(FieldReference field)
        {
            var fieldReference = field;
            var scope = GetAssemblyNameReference(fieldReference.DeclaringType.Scope);
            MapReference(scope, fieldReference);
        }

        public override void VisitMethodReference(MethodReference method)
        {
            var methodReference = method;
            var scope = GetAssemblyNameReference(method.DeclaringType.Scope);
            MapReference(scope, methodReference);
        }

        public override void VisitPropertyReference(PropertyReference property)
        {
            var propertyReference = property;
            var scope = GetAssemblyNameReference(propertyReference.DeclaringType.Scope);
            MapReference(scope, propertyReference);
        }

        public override void VisitTypeReference(TypeReference type)
        {
            if (type.GetElementType() is GenericParameter)
                return;

            var scope = GetAssemblyNameReference(type.Scope);
            MapReference(scope, type);
        }

        private void MapReference(AssemblyNameReference scope, MemberReference reference)
        {
            if (Helper.IsCoreAssemblyName(scope.Name))
                return;

            if (_references.ContainsKey(scope))
                _references[scope].Add(reference);
            else
                _references.Add(scope, new List<MemberReference>() { reference });
        }

        static AssemblyNameReference GetAssemblyNameReference(IMetadataScope scope)
        {
            AssemblyNameReference reference;
            if (scope is ModuleDefinition)
            {
                AssemblyDefinition asm = ((ModuleDefinition)scope).Assembly;
                reference = asm.Name;
            }
            else
                reference = (AssemblyNameReference)scope;

            return reference;
        }
    }
}
