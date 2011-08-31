using System;
using System.Reflection;

namespace Obfuscator.Test.MultipleInterfacesInheritance
{
    public class Class : IBaseARedefinition, IFinal
    {
        #region IBaseARedefinition Members

        public void BaseAMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        #endregion

        #region IFinal Members

        public void FinalMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        #endregion

        #region IMiddle Members

        void IMiddle.MiddleMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        #endregion

        #region IBaseB Members

        // This implementation is only for BaseB, ISameAsBaseB needs to have seperate implementation
        void IBaseB.BaseBMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        #endregion

        #region ISameAsBaseB Members

        public void BaseBMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        #endregion

        public void MiddleMethod()
        {
            PrintMethod(MethodInfo.GetCurrentMethod());
        }

        private static void PrintMethod(MethodBase method)
        {
            Console.WriteLine("Current method: {0}, declaring type: {1}", method, method.DeclaringType.FullName);
        }

    }
}
