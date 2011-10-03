using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;

namespace Obfuscator.Renaming.Reflection
{
    internal static class Map
    {
#if HASH
        private static MD5 hash;
#endif
        private static Dictionary<string, string> NameMap;

        static Map()
        {
#if HASH
            hash = MD5.Create();
#endif
            NameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public static string GetTypeName(string typeFullName)
        {
            return GetName(GetMemberName(typeFullName), typeFullName);
        }

        public static string GetTypeNameFromAssembly(Assembly assembly, string typeFullName)
        {
            var type = assembly.GetType(typeof(Map).FullName);
            var method = type.GetMethod("GetTypeName", new Type[] { typeof(string) });
            return method.Invoke(null, new object[] { typeFullName }) as string;
        }

        public static string GetNestedTypeName(System.Type type, string memberName)
        {
            return GetName(GetNestedTypeKey(type.FullName, GetMemberName(memberName)), memberName);
        }

        public static string GetEventName(System.Type type, string memberName)
        {
            return GetName(GetEventKey(type.FullName, GetMemberName(memberName)), memberName);
        }

        public static string GetFieldName(System.Type type, string memberName)
        {
            return GetName(GetFieldKey(type.FullName, GetMemberName(memberName)), memberName);
        }

        public static string GetMethodName(System.Type type, string memberName, System.Type[] parameters)
        {
            return GetName(GetMethodKey(type.FullName, GetMemberName(memberName), GetParametersString(parameters)), memberName);
        }

        public static string GetPropertyName(System.Type type, string memberName, System.Type[] parameters)
        {
            return GetName(GetPropertyKey(type.FullName, GetMemberName(memberName), GetParametersString(parameters)), memberName);
        }

        private static void AddTypeName(string oldName, string newName)
        {
            NameMap.Add(oldName, newName);
        }

        private static void AddNestedTypeName(string typeName, string oldName, string newName)
        {
            NameMap.Add(GetNestedTypeKey(typeName, oldName), newName);
        }

        private static void AddEventName(string typeName, string oldName, string newName)
        {
            NameMap.Add(GetEventKey(typeName, oldName), newName);
        }

        private static void AddFieldName(string typeName, string oldName, string newName)
        {
            NameMap.Add(GetFieldKey(typeName, oldName), newName);
        }

        private static void AddMethodName(string typeName, string oldName, string parametersString, string newName)
        {
            NameMap.Add(GetMethodKey(typeName, oldName, parametersString), newName);
        }

        private static void AddPropertyName(string typeName, string oldName, string parametersString, string newName)
        {
            NameMap.Add(GetPropertyKey(typeName, oldName, parametersString), newName);
        }

        internal static string GetMemberName(string input)
        {
#if HASH
            return GetMd5Hash(input);
#else
            return input;
#endif
        }

#if HASH
        private static string GetMd5Hash(string input)
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

        static string GetName(string key, string str)
        {
            string result;
            if (NameMap.TryGetValue(key, out result))
                return result;
            return str;
        }

        internal static string GetNestedTypeKey(string typeName, string oldName)
        {
            return String.Concat(typeName, ".N.", oldName);
        }

        internal static string GetFieldKey(string typeName, string oldName)
        {
            return String.Concat(typeName, ".F.", oldName);
        }

        internal static string GetEventKey(string typeName, string oldName)
        {
            return String.Concat(typeName, ".E.", oldName);
        }

        internal static string GetMethodKey(string typeName, string oldName, string parametersString)
        {
            return String.Concat(typeName, ".M.", oldName, parametersString);
        }

        internal static string GetPropertyKey(string typeName, string oldName, string parametersString)
        {
            return String.Concat(typeName, ".P.", oldName, parametersString);
        }

        static string GetParametersString(System.Type[] parameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            if (parameters.Length > 0)
            {
                foreach (var type in parameters)
                {
                    builder.Append(type.FullName);
                    builder.Append(',');
                }
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append(')');
            return builder.ToString();
        }
    }
}

