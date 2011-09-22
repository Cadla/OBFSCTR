using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Diagnostics;

namespace Obfuscator.Renaming
{
    public class CLSNameGenerator : INameGenerator
    {
        IStringGenerator _nameGenerator;
        IDictionary<string, string> _scopeLastNameDictionary;        

        public bool CLSCompliance { get; private set; }
        public bool KeepNamespaces { get; set; }

        public CLSNameGenerator(IStringGenerator nameGenerator, bool clsCompliant, bool keepNamespaces)
        {
            _scopeLastNameDictionary = new Dictionary<string, string>();            
            _nameGenerator = nameGenerator;
            CLSCompliance = clsCompliant;
            KeepNamespaces = keepNamespaces;
        }

        // LOOKUP Path.GetRandomFileName BaseBMethod        
        public string GetMemberName(IMemberDefinition member)
        {
            //Debug.Assert(member.DeclaringType != null);
            
            //string scopeId, newName;            
            ////if (CLSCompliance)
            ////{
            ////    scopeId = GetCLSMemberScope(member);
            ////    newName = GetNextName(scopeId);
            ////    while (newName == declaringTypeName)
            ////        newName = GetNextName(scopeId);
            ////}
            ////else
            ////{
            ////    scopeId = GetCTSMemberScope(member);
            ////    newName = GetNextName(scopeId);
            ////}            
            //return newName + x++;
            return string.Empty;
        }

        public int x = 0;
        public string GetTypeName(TypeDefinition type)
        {
            Debug.Assert(!type.IsNested);

            var scopeId = GetTypeScope(type);
            if (KeepNamespaces)
                return GetNextName(scopeId + "." + type.Namespace);
            else
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
        
        private string GetTypeScope(TypeDefinition type)
        {
            return type.Scope.Name;
        }

        private string GetCTSMemberScope(IMemberDefinition member)
        {            
            StringBuilder scope = new StringBuilder();
            TypeDefinition declaringType = member.DeclaringType;

            char separator;
            if (declaringType.IsNested)
            {
                scope.Append(GetCTSMemberScope(declaringType));
                separator = '/';
            }
            else
            {
                scope.Append(GetTypeScope(declaringType));
                separator = '.';
            }

            scope.Append(separator);
            scope.Append(declaringType.Name);
            scope.Append("::");
            scope.Append(member.MetadataToken.TokenType.ToString());

            MethodDefinition method = member as MethodDefinition;
            if (method != null)
                scope.Append(GetMethodParametersString(method));

            return scope.ToString();
        }

        private string GetCLSMemberScope(IMemberDefinition member)
        {
            var ctsScope = GetCTSMemberScope(member);
            if (member is TypeDefinition || member is MethodDefinition)
                return ctsScope;

            StringBuilder scope = new StringBuilder(ctsScope);

            scope.Append("::");
            FieldDefinition field = member as FieldDefinition;
            if (field != null)
                scope.Append(field.FieldType.FullName);

            PropertyDefinition property = member as PropertyDefinition;
            if (property != null)
                scope.Append(property.PropertyType.FullName);

            EventDefinition @event = member as EventDefinition;
            if (@event != null)
                scope.Append(@event.EventType.FullName);

            return scope.ToString();
        }

        private string GetMethodParametersString(MethodReference method)
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
