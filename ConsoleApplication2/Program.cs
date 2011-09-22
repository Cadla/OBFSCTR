using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ConsoleApplication2
{
    public class ClassX
    {
        public void MethodZ()
        {
            Console.WriteLine("MethodZ");
        }

        public void MethodZ(int x)
        {
            Console.WriteLine("ClassX.MethodZ(int)");
        }
    }

    public class ClassY
    {

        public class ClassNested
        {
            public class ClassNestedNested
            {
                public void MethodZ()
                {

                    Console.WriteLine("Nested Nested MethodZ");
                }

            }
            
            public void MethodZ()
            {

                Console.WriteLine("Nested MethodZ");
            }

        }

        public void MethodZ()
        {
            Console.WriteLine("MethodZ");
        }

        public void MethodZ(int x, int y)
        {

            Console.WriteLine("ClassY.MethodZ(int)");
        }

        public void GenericMethod<Z>(Z x)
        {
            Console.WriteLine("GenricMethod " + x.ToString());
        }

        public static string MethodZ(string i)
        {

            return "Method"+i;
        }

    }

    

    class Program
    {
        static Type[] Method(int i)
        {
            var result = new Type[i];
            for (int j = 0; j < i; j++)
                result[j] = typeof(int);
            return result;
        }

        //NOTE: will not work if classes are moved inside + vs /  

        static void Main(string[] args)
        {
            string method = "MethodZ";
            var x = new ClassX();
            var y = new ClassY();

            var xm = typeof(ClassX).GetMethod(method, Type.EmptyTypes);
            xm.Invoke(x, null);

            // Metody różne przed renamowaniem 
            var ym = y.GetType().GetMethod("MethodZ", Type.EmptyTypes);
            ym.Invoke(y, null);

            var NY = new ClassY.ClassNested();
            var NYNY = new ClassY.ClassNested.ClassNestedNested();
            var ny = typeof(ClassY.ClassNested).GetMethod("Method" + "Z", Type.EmptyTypes);
            var ny2 = Type.GetType("ConsoleApplication2.ClassY").GetNestedType("ClassNested").GetMethod("Method" + "Z", Type.EmptyTypes);
            var nyny = typeof(ClassY.ClassNested.ClassNestedNested).GetMethod("Method" + "Z", Type.EmptyTypes);
            var nyny2 = typeof(ClassY).GetNestedType("ClassNested").GetNestedType("ClassNestedNested").GetMethod("Method" + "Z", Type.EmptyTypes);
            ny.Invoke(NY, null);
            ny2.Invoke(NY, null);
            nyny.Invoke(NYNY, null);
            nyny2.Invoke(NYNY, null);

            //var generic = y.GetType().GetMethod("GenericMethod");
            //generic.MakeGenericMethod(new Type[] { typeof(string) });
            //generic.Invoke(y, new string[] { "abcd"});

            Type[] array;

            var xmp = x.GetType().GetMethod(ClassY.MethodZ("Z"), (array = new Type[] { typeof(int) }));
            xmp.Invoke(x, new object[] { 1 });

            xmp = x.GetType().GetMethod("MethodZ", array);
            xmp.Invoke(x, new object[] { 1 });

            var ymp = y.GetType().GetMethod("MethodZ", new Type[] { typeof(int), typeof(int) });
            ymp.Invoke(y, new object[] { 1, 2 });


            ymp = y.GetType().GetMethod("MethodZ", Method(2));
            ymp.Invoke(y, new object[] { 1, 2 });

            Console.ReadKey();
        }
    }
}
