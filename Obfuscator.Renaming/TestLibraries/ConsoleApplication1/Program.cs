﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace ConsoleApplication1
//{
////    //public interface ICommon
////    //{
////    //    void DoIt();
////    //    void DoIt2();
////    //}


////    public class AClass
////    {
////        public int field1;
////        public string field2;

////        public float Property { get; set; }
////        public object Property2 { get; set; } 

////        public void Foo() { }
       
////        public int Foo2()
////        {
////            return 1;
////        }

////        public static void Foo(string str) { }

////        public static string Foo2(string str)
////        {
////            return str;        
////        }

////        public static int Foo3(string str)
////        {
////            return 1;   
////        }        
////    }




////    public class T<G>
////    {


////    }
////    public class Base {//: ICommon {      
////        //public virtual void DoIt2(string str) { Console.WriteLine("b"); }
////        //public virtual void DoIt(int x) { Console.WriteLine("bb"); }
////        public virtual void DoIt2() { Console.WriteLine("b"); }
////        public virtual void DoIt() { Console.WriteLine("bb"); }
////        public static void A(T<int> x) { }
////        public int B(T<int> x) { return 1; }
////        public static int C(T<int> x) { return 2; }
////        public static void A(T<string> x) { }
////        public static void A(params object[] abc) { }
////    }

////    public class Derived : Base 
////    {
////        //public override void DoIt(int x) { Console.WriteLine("d"); }
////        //public override void DoIt2(string str) { Console.WriteLine("d"); }
////        public override void DoIt() { Console.WriteLine("d"); }
////        public override void DoIt2() { Console.WriteLine("dd"); }
////    }


//    //public class Base
//    //{
//    //    public void MethodB() { Console.WriteLine("Base:MethodB"); }

//    //    public void MethodA() { Console.WriteLine("Base:MethodA"); }
//    //}

//    //public class Derived : Base
//    //{
//    //    public new void MethodA() { Console.WriteLine("Derived:MethodA"); }

//    //    public new void MethodB() { Console.WriteLine("Derived:MethodB"); }
//    //}


////public interface IInterfaceOne
////{
////    void Method();
////}

////public interface IInterfaceTwo
////{
////    void Method();
////}

////public class Class : IInterfaceOne, IInterfaceTwo
////{
////    void IInterfaceOne.Method()
////    {
////        Console.WriteLine("IInterfaceOne:Method");
////    }

////    void IInterfaceTwo.Method()
////    {
////        Console.WriteLine("IInterfaceTwo:Method");
////    }

////    public void Method()
////    {
////        Console.WriteLine("Class:Method");
////    }
////}


//    //public class Base
//    //{
//    //    public virtual void MethodB() { Console.WriteLine("Base:MethodB"); }

//    //    public virtual void MethodA() { Console.WriteLine("Base:MethodA"); }
//    //}

//    //public class Derived : Base
//    //{
//    //    public override void MethodA() { Console.WriteLine("Derived:MethodA"); }

//    //    public override void MethodB() { Console.WriteLine("Derived:MethodB"); }     
//    //}

//    //public class DrivedDerived : Derived
//    //{
//    //    public override void MethodA() { Console.WriteLine("DerivedDerived:MethodA"); }

//    //    public override void MethodB() { Console.WriteLine("DerivedDerived:MethodB"); }     
//    //}

//    class Program
//    {
//        public static void Main()
//        {
//            //DrivedDerived dd = new DrivedDerived();
//            //Derived d = new Derived();
//            //Derived ddd = dd;

//            //Base b = new Base();
//            //Base bd = d;
//            //Base bdd = dd;

//            //b.MethodA();
//            //b.MethodB();

//            //d.MethodA();
//            //d.MethodB();

//            //bd.MethodA();
//            //bd.MethodB();

//            //dd.MethodA();
//            //dd.MethodB();

//            //ddd.MethodA();
//            //ddd.MethodB();
                      
//            //bdd.MethodA();
//            //bdd.MethodB();

////Class c = new Class();
////c.Method();
////((IInterfaceOne)c).Method();
////((IInterfaceTwo)c).Method();


//            //ICommon r4 = r1;
//            //r.DoIt(1);
//            //r.DoIt2("1");

//            //r1.DoIt(1);
//            //r1.DoIt2("1");

//            //r.DoIt();
//            //r.DoIt2();

//            //r1.DoIt();
//            //r1.DoIt2();
                        
//            Console.ReadKey();
//        }
//    }
//}


