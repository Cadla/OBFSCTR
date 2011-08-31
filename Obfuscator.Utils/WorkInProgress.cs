using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Obfuscator.Utils
{
    public static partial class Helper
    {

        public static void FillOverrides(MethodDefinition method)
        {
            if (!method.IsVirtual)
                return;
            
            // for some reason interface method implementations are marked newslot
            MethodReference baseMethod;
            if (!method.IsNewSlot)
                baseMethod = method.GetOriginalBaseMethod();

            IList<MethodReference> interfaceBaseMethods = GetParentMethodFromInterface(method);
        }

        public static IList<MethodReference> GetParentMethodFromInterface(MethodDefinition method)
        {
            var declaringType = method.DeclaringType;
            IList<MethodReference> result = new List<MethodReference>();

            if (declaringType.HasInterfaces)
            {
                foreach (var i in declaringType.Interfaces)
                {
                    var @interface = i.Resolve();
                    var matchingMethod = Helper.GetMethod(@interface.Methods, method);
                    if (matchingMethod != null)
                    {
                        result.Add(matchingMethod);
                    }
                }
            }
            return result;
        }
    }
}
