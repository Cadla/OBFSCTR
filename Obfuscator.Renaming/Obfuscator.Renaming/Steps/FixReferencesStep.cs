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
                //TODO: what if cannot resolve? 
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

    }
    
    public partial class RenameMapVisitor : NullAssemblyVisitor  
    {       

        public override void VisitEventReference(Mono.Cecil.EventReference @event)
        {
            MapReference(@event);
        }


        public override void VisitFieldReference(Mono.Cecil.FieldReference field)
        {
            MapReference(field);
        }

        public override void VisitMethodReference(Mono.Cecil.MethodReference method)
        {
            MapReference(method);
        }

        public override void VisitPropertyReference(Mono.Cecil.PropertyReference property)
        {
            MapReference(property);
        }

        public override void VisitTypeReference(Mono.Cecil.TypeReference type)
        {
            MapReference(type);
        }

        private void MapReference(MemberReference reference)
        {
            _renamer.MapReference(reference);
        } 

    
}

