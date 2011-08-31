using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Test.MultipleInterfacesInheritance
{
    public interface IBaseARedefinition : IBaseA
    {
        new void BaseAMethod();
    }
}
