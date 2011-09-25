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
        IDictionary<string, string> _scopeLastNameDictionary = new Dictionary<string, string>();
        IDictionary<TypeDefinition, string> _typeDefinitionNmaes = new Dictionary<TypeDefinition, string>();

        // <typeScopeId, <methodParametersString, lastName>>
        IDictionary<string, Dictionary<string, List<string>>> _scopedMethodSignatures = new Dictionary<string, Dictionary<string, List<string>>>();

        public bool KeepNamespaces { get; private set; }

        public CLSNameGenerator(IStringGenerator nameGenerator, bool keepNamespaces)
        {            
            _nameGenerator = nameGenerator;
            KeepNamespaces = keepNamespaces;
        }

        // LOOKUP Path.GetRandomFileName BaseBMethod        
        public string GetMemberName(IMemberDefinition member)
        {
            if (member.DeclaringType == null)
                return GetTypeName((TypeDefinition)member);
         
            var declaringTypeName = GetDeclaringTypeName(member.DeclaringType);
            string newName;
            do{
                newName = NewMethod(member);
            }
            while(newName == declaringTypeName);

            //_scopedMemberNames[declaringTypeScope] = newName;
            return newName;
        }

        //TODO: Clean up! Refactor!
        private string NewMethod(IMemberDefinition member)
        {
            string newName = null;
            var declaringTypeScope = GetDeclaringTypeScope(member.DeclaringType);
            if (member.MetadataToken.TokenType != TokenType.Method)
            {
                newName = GetNextName(declaringTypeScope);
            }
            else
            {
                var methodParametersString = GetMethodParametersString((MethodReference)member);
                if (!_scopedMethodSignatures.ContainsKey(declaringTypeScope))
                {
                    newName = GetNextName(declaringTypeScope);
                    _scopedMethodSignatures[declaringTypeScope] = new Dictionary<string, List<string>>()
                    {
                        {methodParametersString, new List<string>() {newName}}
                    };
                }
                else
                {
                    foreach (var method in _scopedMethodSignatures[declaringTypeScope])
                    {
                        if (methodParametersString != method.Key)
                        {
                            List<string> forbidenNames;

                            if (_scopedMethodSignatures[declaringTypeScope].TryGetValue(methodParametersString, out forbidenNames))
                            {
                                foreach (var name in method.Value)
                                    if (!forbidenNames.Contains(name))
                                    {
                                        newName = name;
                                        break;
                                    }
                            }
                            else
                                newName = method.Value.First();

                        }
                    }

                    if (newName == null)
                        newName = GetNextName(declaringTypeScope);
                    if (_scopedMethodSignatures[declaringTypeScope].ContainsKey(methodParametersString))
                        _scopedMethodSignatures[declaringTypeScope][methodParametersString].Add(newName);
                    else
                        _scopedMethodSignatures[declaringTypeScope].Add(methodParametersString, new List<string>() { newName });
                }
            }
            return newName;
        }    
        
        public string GetTypeName(TypeDefinition type)
        {
            if (type.IsNested)
                return GetMemberName(type);

            var scopeId = GetTypeScope(type);
            return _typeDefinitionNmaes[type] = GetNextName(scopeId);            
        }

        private string GetDeclaringTypeName(TypeDefinition typeDefinition)
        {
            string result;
            if (!_typeDefinitionNmaes.TryGetValue(typeDefinition, out result))
                result = GetTypeName(typeDefinition);
            return result;
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
            if (KeepNamespaces)
                return type.Scope.ToString() + "::" + type.Namespace;
            else
                return type.Scope.ToString() + "::";
        }
        
        private string GetDeclaringTypeScope(TypeDefinition declaringType)
        {
            if (declaringType.IsNested)
                return GetDeclaringTypeScope(declaringType.DeclaringType) + '/' + declaringType.Name;
            else
                return GetTypeScope(declaringType) + '.' + declaringType.Name;
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
