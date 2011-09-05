using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Test.Scope
{
    
    // Child class cannot end up having methods with the same name as it's base class
    public class NameConflictBase
    {
        public string C = "BaseC";

        public string A() { return "A"; }

        public string D = "BaseD";

        public string AMethod() { return "AMethod"; }
    }

    public class NameConfictChild : NameConflictBase
    {
        public string A = "ChildA";

        public string C = "ChildC";

        public new string D = "ChildD"; // the new keyword is default anyway

        public string B() { return "B"; }

        public string BMethod() { return "BMethod"; }
    }

    // After Obfuscation they should be named :
    //public class A
    //{
    //    public void A() { }

    //    public void B() { }
    //}

    //public class B : A
    //{
    //    public void C () { }

    //    public void D () { }
    //}
}

