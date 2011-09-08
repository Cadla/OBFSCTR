using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Test
{
    public class SingleClassInheritanceBaseClass
    {
        //public void AMethod() { }

        //public static void StaticMethod() { }

        public virtual void GenericMethod<A>() { }
        public virtual void GenericMethod<A, B>() { }
        public virtual void GenericMethod<A, B, C>() { }

        //public void ImplicitNewInstanceMethod() { }                             // .method public hidebysig instance 
        //public void ExplicitNewInstanceMethod() { }                             // .method public hidebysig instance 
        //// public void ExplicitOverrideInstanceMethod() { }

        //public virtual void ImplicitNewVirtualMethod() { }                      // .method public hidebysig newslot virtual instance 
        //public virtual void ExplicitNewVirtualMethod() { }                      // .method public hidebysig newslot virtual instance 
        //public virtual void ExplicitOverrideVirtualMethod() { }                 // .method public hidebysig newslot virtual instance              

        //public virtual void NewSlotVirtualMethod() { }                          // .method public hidebysig newslot virtual instance              
        //public virtual void VirtualVirtualMethod() { }                          // .method public hidebysig newslot virtual instance              

        //public virtual void SealedVirtualMethod() { }                           // .method public hidebysig newslot virtual instance 

        //public virtual void SkippedVirtualMethod() { }                           // .method public hidebysig newslot virtual instance 
        
        //public virtual void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class SingleClassInheritanceClass : SingleClassInheritanceBaseClass
    {
        //// Implicitly hides static method with the same signature from the base class
        //public void StaticMethod() { }
        //public static void StaticMethod(string str) { }
        //public void StaticMethod(int x) { }

        public void GenericMethod<A>() {}
        public virtual void GenericMethod<A, B>() { }
        public override void GenericMethod<A, B, C>() { }

        //// Implicitly hides base class member of the same name (as if it was preceeded with 'new' keyword
        //public void ImplicitNewInstanceMethod() { }                             // .method public hidebysig instance 
        //public new void ExplicitNewInstanceMethod() { }                         // .method public hidebysig instance 
        //// Only abstract, virtual and override methods can be overriden
        //// public override void ExplicitOverrideInstanceMethod() { }  

        //// Implicitly hides base class member of the same name (as if it was preceeded with 'new' keyword        
        //public void ImplicitNewVirtualMethod() { }                              // .method public hidebysig instance 
        //public new void ExplicitNewVirtualMethod() { }                          // .method public hidebysig instance 
        //public override void ExplicitOverrideVirtualMethod() { }                // .method public hidebysig virtual instance 

        //public new virtual void NewSlotVirtualMethod() { }                      // .method public hidebysig newslot virtual instance 
        //public virtual void VirtualVirtualMethod() { }                          // .method public hidebysig newslot virtual instance              

        //public sealed override void SealedVirtualMethod() { }                   // .method public hidebysig virtual final instance

        //public void Dispose()
        //{

        //}
    }


    public class SingleClassInheritanceChildChild : SingleClassInheritanceClass
    {
        public virtual void GenericMethod<A>() { }
        public override sealed void GenericMethod<A, C>() { }

        //public override void ExplicitOverrideVirtualMethod()
        //{
        //    base.ExplicitOverrideVirtualMethod();
        //}

        //public override void SkippedVirtualMethod()
        //{
        //    base.SkippedVirtualMethod();
        //}

        //public override void NewSlotVirtualMethod()
        //{
        //    base.NewSlotVirtualMethod();
        //}

        //public override void VirtualVirtualMethod()
        //{
        //    base.VirtualVirtualMethod();
        //}

        //public void Dispose()
        //{

        //}
        // a class can override only from a base class
        //public override void SkippedVirtualMethod()
        //{
        //}
    }
    
    public abstract class SingleAbstractClassInheritanceBaseClass
    {
        public abstract void ImplicitNewAbstractMethod();
        public abstract void ExplicitNewAbstractMethod();
        public abstract void ExplicitOverrideAbstractMethod();                  // .method public hidebysig newslot abstract virtual instance 
        public abstract void SealedVirtualMethod();                             // .method public hidebysig newslot abstract virtual instance 
    }

    public abstract class SingleAbstractClassInheritanceClass : SingleAbstractClassInheritanceBaseClass
    {
        // public void ImplicitNewAbstractMethod() { } // Not abstract class have to implement all of the abstract methods explicitly
        // public new void ExplicitNewAbstractMethod() { } // New method   
        public abstract override void ExplicitNewAbstractMethod();
        public override void ExplicitOverrideAbstractMethod() { }               // .method public hidebysig virtual instance 
        public sealed override void SealedVirtualMethod() { }                   // .method public hidebysig virtual final instance      
    }

    public class SingleAbstractClassInteritanceChildChild : SingleAbstractClassInheritanceClass
    {
        public override void ImplicitNewAbstractMethod()
        {
            throw new NotImplementedException();
        }

        public override void ExplicitNewAbstractMethod()
        {
            throw new NotImplementedException();
        }
    }
}

