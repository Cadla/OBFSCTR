using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Obfuscator.Test.MultiLevelClassInheritance
{
    public class MiddleClass : AbstractBase
    {
        public new void InstanceMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        public override sealed void AbstractMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        public new void StaticMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }
    }
}
