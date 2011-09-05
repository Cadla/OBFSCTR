using System;
using System.Collections.Generic;
using Mono.Cecil;
using Obfuscator.Utils;

namespace Obfuscator.Renaming
{
    public partial class RenameVisitor : NullAssemblyVisitor
    {
        public IDictionary<string, string> _renameMap = new Dictionary<string, string>();

        // TODO read: http://www.unicode.org/unicode/reports/tr15/tr15-18.htm to get allowed characters
        NameGenerator _nameGenerator = new NameGenerator("ABCDEFGHIJKLMNOPRSTUWXYZ");
        public ScopeSth _scope;

        const string SKIPPING_IGNORED = "IS IGNORRED";
        const string SKIPPING_CONSTRUCTOR = "IS A CONSTRUCTOR"; // TODO isn't constructor a special name
        const string SKIPPING_SPECIAL_NAME = "IS A SPECIAL NAME";
        const string SKIPPING_RUNTIME = "IS RUNTIME";
        const string SKIPPING_EXTERNAL_VIRTUAL = "IS EXTERNAL VIRTUAL";

        // TODO check virtual methods (Linker?)
        // TODO renaming references in different assemblies
        // TODO what's a relationship between types and resources
        // TODO member names cannot be the same as their enclosing types

        partial void logVisitingMember(IMemberDefinition member);
        partial void logSkipingMember(IMemberDefinition member, string message);


        public RenameVisitor()            
        {
            _scope = new ScopeSth(_nameGenerator);
        }

        public void PringMap()
        {
            foreach (var r in _renameMap)
                Console.WriteLine(r);
        }

        public override void VisitTypeDefinition(Mono.Cecil.TypeDefinition type)
        {
            if (!ShouldBeRenamed(type))
                return;
            
            logVisitingMember(type);
            Rename(type);            
        }

        public override void VisitMethodDefinition(Mono.Cecil.MethodDefinition method)
        {                            
            if (!ShouldBeRenamed(method))
                return;
            
            logVisitingMember(method);
            Rename(method);
        }

        public override void VisitFieldDefinition(Mono.Cecil.FieldDefinition field)
        {
            if (!ShouldBeRenamed(field))
                return;
            
            logVisitingMember(field);
            Rename(field);            
        }

        public override void VisitEventDefinition(Mono.Cecil.EventDefinition @event)
        {
            if (!ShouldBeRenamed(@event))
                return;

            logVisitingMember(@event);
            Rename(@event);            
        }

        public override void VisitPropertyDefinition(Mono.Cecil.PropertyDefinition property)
        {
            if (!ShouldBeRenamed(property))            
                return;

            logVisitingMember(property);
            Rename(property);            
        }

        //TODO Same fully-qualified type names in two assemblies http://msdn.microsoft.com/en-us/library/ms173212.aspx

        private void Rename(IMemberDefinition member)
        {
            string oldName = member.FullName;            
            string newName = _scope.GetName(member);

            TypeDefinition type = member as TypeDefinition;
            if (type != null)
            {
                member.Name = newName;

                // Renaming namespace                                            
                //      type.Namespace = _nameGenerator.GetName(type.Scope.Name);                       
            }
            else
            {
                member.Name = newName;                
            }

            // validates uniqueness
            _renameMap[oldName] = member.FullName;
        }

        private bool ShouldBeRenamed(MethodDefinition method)
        {
            if (IsIgnored(method)){
                logSkipingMember(method, SKIPPING_IGNORED);
                return false;
            }

            if (method.IsConstructor)
            {
                logSkipingMember(method, SKIPPING_CONSTRUCTOR);
                return false;
            }
            if (method.IsSpecialName)
            {
                logSkipingMember(method, SKIPPING_SPECIAL_NAME);
                return false;
            }
            if (method.IsRuntime)
            {
                logSkipingMember(method, SKIPPING_RUNTIME);
                return false;
            }
            if (IsVirtualImplementation(method))
            {
                logSkipingMember(method, SKIPPING_EXTERNAL_VIRTUAL);
            }
             
            return true;
        }

        private bool IsVirtualImplementation(MethodDefinition method)
        {
 
           // TODO sprawdz czy nalezy dodac cos do .override
           // if(method.)
            return false;
        }

        private bool ShouldBeRenamed(TypeDefinition type)
        {
            if (IsIgnored(type))
                return false;

            if (type.Name == "<Module>")
                return false;

            return true;
        }

        private bool ShouldBeRenamed(FieldDefinition field)
        {
            if (IsIgnored(field))
                return false;

            if (field.IsSpecialName)
            {
                logSkipingMember(field, SKIPPING_SPECIAL_NAME);
                return false;
            }

   
            return true;
        }

        private bool ShouldBeRenamed(PropertyDefinition property)
        {
            if (IsIgnored(property))
                return false;

            return true;
        }

        private bool ShouldBeRenamed(EventDefinition @event)
        {
            if (IsIgnored(@event))
                return false;

 
            return true;
        }


        private bool IsIgnored(MethodDefinition member)
        {
            if (member.Name == "Main")
                return true;
            return false;
        }

        private bool IsIgnored(IMemberDefinition member)
        {

            return false;
        }
        
    }
}
