using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Renaming
{
    public class ScopeNameGenerator
    {
        INameGenerator _nameGenerator;
        IDictionary<string, string> _scopeLastNameDictionary = new Dictionary<string, string>();

        public ScopeNameGenerator(INameGenerator nameGenerator)
        {
            _nameGenerator = nameGenerator;
        }

        // LOOKUP Path.GetRandomFileName BaseBMethod        
        //Types, fields, methods, properties, and events have names
        // TODO member names cannot be the same as their enclosing types
        public string GetName(IMemberDefinition member)
        {
            var scopeId = GetScope(member);

             // type cannot get a name of it's declaring type
            // while (type.IsNested && newName == type.DeclaringType.Name)
            //  newName = 
            return GetNextName(scopeId);
        }
        
        private string GetNextName(string scopeId)
        {
            string nextName;
            if (!_scopeLastNameDictionary.ContainsKey(scopeId))
            {
                nextName = _nameGenerator.GetStartingString();                
            }
            else
            {
                var lastName = _scopeLastNameDictionary[scopeId];
                nextName = _nameGenerator.GetNext(lastName);
            }
            _scopeLastNameDictionary[scopeId] = nextName;
            return nextName;
        }

        //TODO check wheter library.dll and library.exe are different scope names 
        // char separator = type.DeclaringType.IsNested ? '/' : '.';
        // scope of a type is it's assembly
        private static string GetTypeScope(TypeDefinition type)
        {
            if (type.IsNested)
            {
                char separator = type.DeclaringType.IsNested ? '/' : ':';
                return GetTypeScope(type.DeclaringType) + separator + type.DeclaringType.Name;
            }
            // TODO to remove namespaces treat take type's full name
            // The type name is said to be in the assembly scope of
            // the assembly that implements the type. Assemblies themselves have names that form the basis of the

            return type.Scope.Name;
        }

        //private static string GetTypeMemberScope

        // a scope of a type member is the type that defines the member
        private static string GetScope(IMemberDefinition member)
        {
            // The tuple of (member name, member kind, and member
            // signature) is unique within a member scope of a type.
            TypeDefinition type = member as TypeDefinition;

            if (type != null)
                return GetTypeScope(type);

            StringBuilder scope = new StringBuilder();
            TypeDefinition declaringType = member.DeclaringType;
            scope.Append(GetTypeScope(declaringType));
            char separator = member.DeclaringType.IsNested ? '/' : '.';
            scope.Append(separator);
            scope.Append(declaringType.Name);
            scope.Append("::");
            scope.Append(member.MetadataToken.TokenType.ToString());

            MethodDefinition method = member as MethodDefinition;
            if (method != null)
            {
                scope.Append(GetMethodParametersString(method));
                return scope.ToString();
            }
            
            FieldDefinition field = member as FieldDefinition;
            if(field != null)
            {                
                scope.Append("::");
                scope.Append(field.FieldType.FullName);                
                return scope.ToString();
            }

            PropertyDefinition property = member as PropertyDefinition;
            if (property != null)
            {
                scope.Append("::");
                scope.Append(property.PropertyType.FullName);
                return scope.ToString();
            }

            EventDefinition @event = member as EventDefinition;
            scope.Append("::");
            scope.Append(@event.EventType.FullName);
            return scope.ToString();
        }

        private static string GetMethodParametersString(MethodReference method)
        {            
            if (method.HasParameters)
            {
                StringBuilder list = new StringBuilder();
                list.Append("(");
                foreach (var param in method.Parameters)
                {
                    list.Append(param.ParameterType.FullName + ",");
                }
                list.Remove(list.Length - 1, 1);
                list.Append(")");
                return list.ToString();
            }
            return "()";
        }
    }
}
