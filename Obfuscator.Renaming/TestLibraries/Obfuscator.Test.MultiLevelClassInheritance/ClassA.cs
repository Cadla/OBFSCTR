using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Obfuscator.Test.MultiLevelClassInheritance
{
    public class ClassA : MiddleClass
    {
        public override void VirtualMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
            
        }

        public virtual void AbstractMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        public virtual void ClassAVirtualMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }
    }
}
