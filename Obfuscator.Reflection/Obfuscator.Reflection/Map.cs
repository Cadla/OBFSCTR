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
            return GetName(typeFullName, GetKey(typeFullName));
        }

        public static string GetMember(System.Type type, string memberName)
        {            
            return GetName(memberName, GetKey(type.FullName, memberName));
        }

        public static string GetMemberWithParameters(System.Type type, string memberName, System.Type[] parameters)
        {
            return GetName(memberName, GetKey(type.FullName, memberName, parameters));
        }

        private static void AddType(string oldName, string newName)
        {
            NameMap.Add(GetKey(oldName), newName);
        }

        private static void AddMember(string typeName, string oldName, string newName)
        {
            NameMap.Add(GetKey(typeName, oldName), newName);
        }

        private static void AddMemberWithParameters(string typeName, string oldName, string parametersString, string newName)
        {            
            NameMap.Add(GetKey(typeName, oldName, parametersString), newName);
        }

        static string GetMd5Hash(string input)
        {
            byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }     

        static string GetName(string str, string key)
        {
            //string hashed = GetMd5Hash(str);
            string result;
            if (NameMap.TryGetValue(key, out result))
                return result;
            return str;
        }

        private static string GetKey(string typeName)
        {
            return typeName;
        }

        static string GetKey(string typeName, string oldName)
        {
            return String.Concat(GetKey(typeName), typeMemberSeparator, oldName);
        }

        static string GetKey(string typeName, string oldName, System.Type[] parameters)
        {
            return GetKey(typeName, oldName, GetParametersString(parameters));
        }

        static string GetKey(string typeName, string oldName, string parametersString)
        {
            return String.Concat(GetKey(typeName, oldName), parametersString);
        }

        internal static string GetParametersString(System.Type[] parameters)
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

