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
        public void MethodZ()
        {
            Console.WriteLine("MethodZ");
        }

        public void MethodZ(int x, int y)
        {

            Console.WriteLine("ClassY.MethodZ(int)");
        }

        public void MethodZ<Z>(Z x)
        {

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
            var x = new ClassX();
            var y = new ClassY();

            var xm = typeof(ClassX).GetMethod("MethodZ", Type.EmptyTypes);
            xm.Invoke(x, null);

            // Metody różne przed renamowaniem 
            var ym = y.GetType().GetMethod("MethodZ", Type.EmptyTypes);
            ym.Invoke(y, null);

            Type[] array;

            var xmp = x.GetType().GetMethod("MethodZ", (array = new Type[] { typeof(int) }));
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
