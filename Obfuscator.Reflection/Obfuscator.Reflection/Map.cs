using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Mono.Cecil;
using Obfuscator.Utils;
using Mono.Cecil.Cil;
using Obfuscator.MetadataBuilder.Extensions;
using Obfuscator;

namespace Obfuscator.Reflection
{
    internal static class Map
    {
        private static MD5 hash;
        private static Dictionary<string, string> NameMap;
        private const string typeMemberSeparator = "::";

        static Map()
        {
            hash = MD5.Create();
            NameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);       
        }
        
        public static string GetType(string typeFullName)
        {
#if HASH
            return GetName(typeFullName, GetMd5Hash(typeFullName));
#else
            return GetName(typeFullName, typeFullName);
#endif
        }

        public static string GetMember(System.Type type, string memberName)
        {            
#if HASH
            return GetName(memberName, GetKey(type.FullName, GetMd5Hash(memberName)));
#else
            return GetName(memberName, GetKey(type.FullName, memberName));
#endif
        }

        public static string GetMemberWithParameters(System.Type type, string memberName, System.Type[] parameters)
        {
#if HASH
            return GetName(memberName, GetKey(type.FullName, GetMd5Hash(memberName), parameters));
#else
            return GetName(memberName, GetKey(type.FullName, memberName, parameters));
#endif
        }

        private static void AddType(string oldName, string newName)
        {
            NameMap.Add(oldName, newName);
        }

        private static void AddMember(string typeName, string oldName, string newName)
        {
            NameMap.Add(GetKey(typeName, oldName), newName);
        }

        private static void AddMemberWithParameters(string typeName, string oldName, string parametersString, string newName)
        {            
            NameMap.Add(GetKey(typeName, oldName, parametersString), newName);
        }
#if HASH
        internal static string GetMd5Hash(string input)
        {
            byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }     
#endif

        static string GetName(string str, string key)
        {            
            string result;
            if (NameMap.TryGetValue(key, out result))
                return result;
            return str;
        }

        static string GetKey(string typeName, string oldName)
        {
            return String.Concat(typeName, typeMemberSeparator, oldName);
        }

        static string GetKey(string typeName, string oldName, System.Type[] parameters)
        {
            return GetKey(typeName, oldName, GetParametersString(parameters));
        }

        static string GetKey(string typeName, string oldName, string parametersString)
        {
            return String.Concat(GetKey(typeName, oldName), parametersString);
        }

        static string GetParametersString(System.Type[] parameters)
        {
            StringBuilder builder = new StringBuilder();
            if (parameters.Length > 0)
            {
                builder.Append('(');
                foreach (var type in parameters)
                {
                    builder.Append(type.FullName);
                    builder.Append(',');
                }
                builder.Remove(builder.Length - 1, 1);
                builder.Append(')');
                return builder.ToString();
            }
            else return "()";
        }
    }
}

