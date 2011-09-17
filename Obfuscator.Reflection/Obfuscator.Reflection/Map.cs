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

namespace Reflection
{
    internal static class MapInjector
    {        
        
    }

    internal static class Map
    {
        private static MD5 hash;
        private static Dictionary<string, string> NameMap;

        private static void Add(string oldName, string newName)
        {
            //NameMap.Add(GetMd5Hash(oldName), newName);
            NameMap.Add(oldName, newName);
        }

        static Map()
        {
            hash = MD5.Create();
            NameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);       
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

        public static string Get(string str)        
        {
            string result;
            //string hashed = GetMd5Hash(str);
            string hashed = str;
            if (NameMap.TryGetValue(hashed, out result))
                return result;
            return str;
        }

   
    }
}

