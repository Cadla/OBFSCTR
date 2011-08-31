using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Obfuscator.Test.MultiLevelClassInheritance
{
    public abstract class AbstractBase
    {
        public abstract void AbstractMethod();

        public void InstanceMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        public virtual void VirtualMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        public static void StaticMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        protected static void PrintMethod(MethodBase method)
        {
            Console.WriteLine("Current method: {0}, declaring type: {1}", method, method.DeclaringType.FullName);
        }
    }
}
