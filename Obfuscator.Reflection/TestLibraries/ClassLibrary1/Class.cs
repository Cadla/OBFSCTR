using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Reflection
{
    public class Class
    {
        public string Method()
        {
            return "Method";
        }
    }

    public class RenamedClass
    {
        public string RenamedMethod()
        {
            return "RenamedMethod";
        }
    }

    public class X
    {
        static X()
        {
            MethodName = "Method";
        }

        private static string methodName = "Method";

        public static string MethodName { get; set; }

        public static string GetMethodName()
        {
            return "Method";
        }

        public static void Reflection(string mName)
        {
            // NOTE class is loaded with instruction ldtoken, doesn't have to be handled
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(Class));
            // TODO ignoreCase flage
            // NOTE types - only fullname

            var type = assembly.GetType("Reflection.Class");

            var method = type.GetMethod("Method");
            //var method2 = type.GetMethod(methodName);
            //var method3 = type.GetMethod(MethodName);
            //var method4 = type.GetMethod(mName);
            //var method5 = type.GetMethod(GetMethodName());

            var instance = assembly.CreateInstance("Reflection.Class");

            Console.WriteLine(method.Invoke(instance, null));
            //Console.WriteLine(method2.Invoke(cl, null));
            //Console.WriteLine(method3.Invoke(cl, null));
            //Console.WriteLine(method4.Invoke(cl, null));
            //Console.WriteLine(method5.Invoke(cl, null));

            // PLAN : REnamowanie wszystkich stringow
            // Opracowac mechanizm przechwytujacy stringi przekazywane do wybranych metod. Stringi te są szyfrowane i sprawdzane z mapą
            // mapa zawiera zaszyfrowane nazwy i ich odpowiedniki w kodzie.
        }
    }
}
