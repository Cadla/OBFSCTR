using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Renaming
{
    public class ScopeSth
    {
        INameGenerator _nameGenerator;       
        IDictionary<string, string> names = new Dictionary<string, string>();

        public ScopeSth(INameGenerator nameGenerator)
        {
            _nameGenerator = nameGenerator;

        }

        // LOOKUP Path.GetRandomFileName BaseBMethod        
        //Types, fields, methods, properties, and events have names
        public string GetName(IMemberDefinition member)
        {
            var scopeId = GetScope(member);

            if (!names.ContainsKey(scopeId))
            {
                names[scopeId] = "";
            }

            var newName = _nameGenerator.GetNext(names[scopeId]);
            names[scopeId] = newName;
            // type cannot get a name of it's declaring type
            // while (type.IsNested && newName == type.DeclaringType.Name)
              //  newName = 
            return newName;
        }

        //TODO check wheter library.dll and library.exe are different scope names 
        // char separator = type.DeclaringType.IsNested ? '/' : '.';
        // scope of a type is it's assembly
        private static string GetTypeScope(TypeDefinition type)
        {
            if (type.IsNested)
            {
                char separator = type.DeclaringType.IsNested ? '/' : '.';
                return GetScope(type.DeclaringType) + separator + type.DeclaringType.Name;
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
                scope.Append(GetMethodParametersString(method));

            return scope.ToString();
        }

        private static string GetMethodParametersString(MethodReference method)
        {
            StringBuilder list = new StringBuilder();
            if (method.HasParameters)
            {
                list.Append("(");
                foreach (var param in method.Parameters)
                {
                    list.Append(param.ParameterType.FullName + ",");
                }
                list.Remove(list.Length - 1, 1);
                list.Append(")");
            }
            return list.ToString();
        }
    }
}
