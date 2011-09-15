using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Test.SingleClassInheritance
{
    //public interface IFirstLevelClass : IBaseClass
    //{
    //    void Method();
    //    void Method2();
    //}

    //public interface IBaseClass
    //{
    //    void Method();
    //}

    //public class BaseClass : IBaseClass
    //{
    //    public virtual void Method()
    //    {
    //        Console.WriteLine("BaseClass {0}, {1}", this.GetType(), "Method");
    //    }

    //    public virtual void Method2()
    //    {
    //        Console.WriteLine("BaseClass {0}, {1}", this.GetType(), "Method2");
    //    }
    //}

    //public class FirstLevelClass : IFirstLevelClass
    //{
    //    public void Method()
    //    {
    //        Console.WriteLine("FirstLevelClass {0}, {1}", this.GetType(), "Method");
    //    }

    //    public void Method2()
    //    {
    //        Console.WriteLine("FirstLevelClass {0}, {1}", this.GetType(), "Method2");
    //    }
    //}

    public interface IClass
    {
        string Method();
    }

    public abstract class Class : IClass
    {
        public abstract string Method();        
    }

    public class Child : Class
    {
        public override string Method()
        {
            return "Child implementation";
        }
    }
}
