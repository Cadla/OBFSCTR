using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    //public interface ICommon
    //{
    //    void DoIt();
    //    void DoIt2();
    //}


    public class AClass
    {
        public int field1;
        public string field2;

        public float Property { get; set; }
        public object Property2 { get; set; } 

        public void Foo() { }
       
        public int Foo2()
        {
            return 1;
        }

        public static void Foo(string str) { }

        public static string Foo2(string str)
        {
            return str;        
        }

        public static int Foo3(string str)
        {
            return 1;   
        }        
    }




    public class T<G>
    {


    }
    public class Base {//: ICommon {      
        //public virtual void DoIt2(string str) { Console.WriteLine("b"); }
        //public virtual void DoIt(int x) { Console.WriteLine("bb"); }
        public virtual void DoIt2() { Console.WriteLine("b"); }
        public virtual void DoIt() { Console.WriteLine("bb"); }
        public static void A(T<int> x) { }
        public int B(T<int> x) { return 1; }
        public static int C(T<int> x) { return 2; }
        public static void A(T<string> x) { }
        public static void A(params object[] abc) { }
    }

    public class Derived : Base 
    {
        //public override void DoIt(int x) { Console.WriteLine("d"); }
        //public override void DoIt2(string str) { Console.WriteLine("d"); }
        public override void DoIt() { Console.WriteLine("d"); }
        public override void DoIt2() { Console.WriteLine("dd"); }
    }



    class Program
    {
        public static void Main()
        {
            Derived r = new Derived();
            Base r1 = r;
            //ICommon r4 = r1;
            //r.DoIt(1);
            //r.DoIt2("1");

            //r1.DoIt(1);
            //r1.DoIt2("1");

            r.DoIt();
            r.DoIt2();

            r1.DoIt();
            r1.DoIt2();
                        
            Console.ReadKey();
        }
    }
}
