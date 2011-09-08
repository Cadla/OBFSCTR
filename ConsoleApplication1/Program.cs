using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Test.MultiLevelClassInheritance;

namespace ConsoleApplication1
{
    class Program
    {
        //public class B
        //{
        //    public virtual void M()
        //    {

        //    }
        //}

        //public class C : B
        //{
        //    public virtual void M()
        //    {

        //    }
        //}

        //public class CC : C
        //{
        //    public override void M()
        //    {

        //    }
        //}

        //public abstract class B
        //{
        //    public abstract void M();
        //}

        //public abstract class C : B
        //{
        //    public abstract void M();
        //}

        // Abstract methods cannot hide itself

        //public class CC : C
        //{
        //    public override void M()
        //    {

        //    }

        //    //public void M()
        //    //{

        //    //}
        //}

        //public interface B
        //{
        //    void M();
        //}

        //public interface C : B
        //{
        //    void M();
        //}

        //public class CC : C
        //{
        //    public void M()
        //    {

        //    }

        //    void C.M()
        //    {

        //    }
        //}


        //public interface IClass
        //{
        //    string Method();
        //}

        //public abstract class Class : IClass
        //{
        //    public abstract string Method();
        //}

        //public class Child : Class
        //{
        //    public override string Method()
        //    {
        //        Console.WriteLine("Child implementation");
        //        return "";                        
        //    }
        //}

        static void Main(string[] args)
        {
            //IClass c = new Child();
            //c.Method();

            //Class cc = new Child();
            //cc.Method();

            var classA = new ClassA();
            classA.StaticMethod();
            classA.AbstractMethod();
            classA.ClassAVirtualMethod();
            classA.InstanceMethod();
            classA.VirtualMethod();


            var middleClass = new MiddleClass();
            middleClass.AbstractMethod();
            middleClass.InstanceMethod();
            middleClass.StaticMethod();
            middleClass.VirtualMethod();

            AbstractBase b = classA;

            b.AbstractMethod();
            b.InstanceMethod();
            b.VirtualMethod();

            b = middleClass;

            b.AbstractMethod();
            b.InstanceMethod();
            b.VirtualMethod();
            
            

            Console.ReadKey();
        }
    }
}
