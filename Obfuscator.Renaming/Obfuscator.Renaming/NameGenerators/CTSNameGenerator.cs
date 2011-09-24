using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Diagnostics;

namespace Obfuscator.Renaming
{
    public class CTSNameGenerator : INameGenerator
    {
        IStringGenerator _nameGenerator;
        
        IDictionary<string, string> _typeNames = new Dictionary<string, string>();

        //IDictionary<string, string> _eventNames = new Dictionary<string, string>();        
        //IDictionary<string, string> _fieldNames = new Dictionary<string, string>();
        //IDictionary<string, string> _methodNames = new Dictionary<string, string>();
        //IDictionary<string, string> _propertyNames = new Dictionary<string, string>();
        
        public bool KeepNamespaces { get; private set; }

        public CTSNameGenerator(IStringGenerator nameGenerator, bool keepNamespaces)
        {
            KeepNamespaces = keepNamespaces;
            _nameGenerator = nameGenerator;            
        }

        // LOOKUP Path.GetRandomFileName BaseBMethod        
        public string GetMemberName(IMemberDefinition member)
        {
            if (member.DeclaringType == null)
                return GetTypeName((TypeDefinition)member);
                         
            var scopeId = GetMemberScope(member);            
            return GetNextName(scopeId);
        }
        
        public string GetTypeName(TypeDefinition type)
        {
            if (type.IsNested)
                return GetMemberName(type);

            var scopeId = GetTypeScope(type);
            return GetNextName(scopeId);            
        }

        private string GetNextName(string scopeId)
        {
            string nextName;
            if (_typeNames.ContainsKey(scopeId))
            {
                var lastUsedName = _typeNames[scopeId];
                nextName = _nameGenerator.GetNext(lastUsedName);
            }
            else
                nextName = _nameGenerator.GetStartingString();

            _typeNames[scopeId] = nextName;
            return nextName;
        }

        private string GetTypeScope(TypeDefinition type)
        {
            if (KeepNamespaces)
                return type.Scope.ToString() + "::" + type.Namespace;
            else
                return type.Scope.ToString() + "::";
        }

        private string GetMemberScope(IMemberDefinition member)
        {
            StringBuilder scope = new StringBuilder();

            scope.Append(GetDeclaringTypeScope(member.DeclaringType));
            
            scope.Append("::");
            scope.Append(member.MetadataToken.TokenType.ToString());

            scope.Append("::");
            scope.Append(GetMemberSignature(member));
            return scope.ToString();
        }
        
        private string GetDeclaringTypeScope(TypeDefinition declaringType)
        {
            if (declaringType.IsNested)
                return GetMemberScope(declaringType) + '/' + declaringType.Name;
            else
                return GetTypeScope(declaringType) + '.' + declaringType.Name;
        }

        private string GetMemberSignature(IMemberDefinition member)
        {
            switch (member.MetadataToken.TokenType)
            {
                case TokenType.Method:
                    return GetMethodParametersString((MethodDefinition)member);                    
                case TokenType.Field:                    
                    return GetTypeSignature(((FieldDefinition)member).FieldType);
                case TokenType.Property:
                    return GetTypeSignature(((PropertyDefinition)member).PropertyType);
                case TokenType.Event:
                    return GetTypeSignature(((EventDefinition)member).EventType);
                default:
                    return String.Empty;
            }
        }

        private string GetMethodParametersString(MethodReference method)
        {
            if (method.HasParameters)
            {
                StringBuilder list = new StringBuilder();
                list.Append('(');
                foreach (var param in method.Parameters)
                {
                    list.Append(GetTypeSignature(param.ParameterType));
                    list.Append(',');
                }
                list.Remove(list.Length - 1, 1);
                list.Append(')');
                return list.ToString();
            }
            return "()";
        }

        private static string GetTypeSignature(TypeReference type)
        {
            //if (type.IsGenericParameter)
            //    return "GEN";
            return type.FullName;
        }
    }
}
