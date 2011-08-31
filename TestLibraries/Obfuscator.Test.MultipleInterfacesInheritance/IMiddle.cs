using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Test.MultipleInterfacesInheritance
{
    public interface IMiddle : IBaseA, IBaseB
    {
        void MiddleMethod();

        void BaseAMethod();
    }
}
