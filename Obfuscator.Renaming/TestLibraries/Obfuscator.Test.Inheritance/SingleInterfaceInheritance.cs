using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Test
{
    public interface ISingleInterfaceInheritance
    {
        void InterfaceMethod();                                     // .method public hidebysig newslot abstract virtual instance 
    }

    public class SingleInterfaceInheritance : ISingleInterfaceInheritance
    {
        // Implicit interface implementation
        public void InterfaceMethod() { }                           // .method public hidebysig newslot virtual final instance
        
    }

    public class SingleInterfaceInheritanceExplicit : ISingleInterfaceInheritance
    {
        // Is not a virtual method
        public void InterfaceMethod() { }                           // .method public hidebysig instance 

       //  private override sealed void InterfaceMethod() { }                           // has to be public
        // Explicit interface implementation
        void ISingleInterfaceInheritance.InterfaceMethod() { }      // .method private hidebysig newslot virtual final instance 
                                                                    // override Obfuscator.Test.ISingleInterfaceInheritance::InterfaceMethod


        // If interface method is explicitly overriden then other methdos are ignored. Interface method implementation is private and the otehr implementation hides 
        // the public Interface method so that it cannot be implicitly invoked
    }
    
    // TODO The .override directive specifies that a virtual method shall be implemented (overridden), in this type, by a 
    // virtual method with a different name, but with the same signature. NOT THE SAME NAME !

    // TODO http://msdn.microsoft.com/en-us/library/ms173157.aspx - on access modifiers http://msdn.microsoft.com/en-us/library/aa664591(v=vs.71).aspx
    // TODO interfaces can have properties (suprisingly). 
}
