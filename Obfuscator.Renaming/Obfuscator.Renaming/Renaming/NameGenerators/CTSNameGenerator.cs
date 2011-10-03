using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Diagnostics;

namespace Obfuscator.Renaming.NameGenerators
{
    public class CTSNameGenerator : INameGenerator
    {
        bool _keepNamespaces;
        StringGenerator _nameGenerator;        
        IDictionary<string, string> _typeNames = new Dictionary<string, string>();

        public CTSNameGenerator(StringGenerator nameGenerator, bool keepNamespaces)
        {
            _keepNamespaces = keepNamespaces;
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
            if (_keepNamespaces)
                return type.Scope.ToString() + "::" + type.Namespace;
            else
                return type.Scope.ToString() + "::";
        }

        // A member name needs to be unique only among code entities that have exactly the same signature
        // and are declared by the same type
        private string GetMemberScope(IMemberDefinition member)
        {
            StringBuilder scope = new StringBuilder();

            scope.Append(GetDeclaringTypeScope(member.DeclaringType));            
            scope.Append(".");

            AppendMemberSignature(scope, member);
            return scope.ToString();
        }
        
        private string GetDeclaringTypeScope(TypeDefinition declaringType)
        {
            if (declaringType.IsNested)
                return GetMemberScope(declaringType) + '/' + declaringType.Name;
            else
                return GetTypeScope(declaringType) + '.' + declaringType.Name;
        }

        private void AppendMemberSignature(StringBuilder scope, IMemberDefinition member)
        {
            scope.Append(member.MetadataToken.TokenType.ToString());
            scope.Append(' ');            
            switch (member.MetadataToken.TokenType)
            {
                case TokenType.Method:
                    // CTS enambles overloading on the full signature, return type, calling convention etc.
                    AppendMethodSignature(scope, (MethodDefinition)member);
                    break;                
                case TokenType.Field:
                    // The calling convention is same for all fields, thus a field's signature consists only
                    // of it's type
                     AppendTypeSignature(scope, ((FieldDefinition)member).FieldType);
                     break;
                case TokenType.Property:
                    AppendPropertySignature(scope, (PropertyDefinition)member);
                    break;
                case TokenType.Event:
                    AppendTypeSignature(scope, ((EventDefinition)member).EventType);
                    break;
            }
        }

        private static void AppendPropertySignature(StringBuilder scope, PropertyDefinition property)
        {
            AppendTypeSignature(scope, property.PropertyType);
            scope.Append(' ');
            AppendParametersString(scope, property.Parameters);
        }

        //TODO: Consider creating a class that will represent member signature
        private static void AppendMethodSignature(StringBuilder scope, IMethodSignature method)
        {
            scope.Append(method.HasThis ? 's' : 'i');
            scope.Append(' ');
            scope.Append(method.CallingConvention.ToString());
            
            scope.Append(' ');
            AppendTypeSignature(scope, method.ReturnType);
            scope.Append(' ');
            AppendParametersString(scope, method.Parameters);
        }

        private static void AppendParametersString(StringBuilder scope, IList<ParameterDefinition> parameters)
        {
            scope.Append('(');
            if (parameters.Count != 0)
            {                                
                foreach (var param in parameters)
                {
                    AppendTypeSignature(scope, param.ParameterType);
                    scope.Append(',');
                }
                scope.Remove(scope.Length - 1, 1);                
            }
            scope.Append('(');
        }


        private static void AppendTypeSignature(StringBuilder scope, TypeReference type)
        {  
            scope.Append(type.FullName);
        }
    }
}

