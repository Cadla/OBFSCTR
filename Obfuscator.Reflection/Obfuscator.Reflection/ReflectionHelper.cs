using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Reflection
{
    class ReflectionHelper
    {
        //protected static IEnumerable<KeyValuePair<Method, int>> ReflectionMethods()
        //{
        //    ModuleDefinition corlib = ModuleDefinition.ReadModule(typeof(object).Module.FullyQualifiedName);
        //    var typeType = corlib.GetType("System.Type");
        //    var methods = typeType.GetMethods().Where(m => m.HasParameters && m.Parameters[0].ParameterType.FullName == "System.String");
        //    var assemblyType = corlib.GetType("System.Reflection.Assembly");
        //    var assemblyMethods = assemblyType.GetMethods().Where(m => m.HasParameters && m.Parameters[0].ParameterType.FullName == "System.String");

        //    var reflectionMethods = methods.Union(assemblyMethods).Cast<MethodReference>().ToList();
        //    return reflectionMethods.Select(m => new KeyValuePair<Method, int>(new Method(m), 1));
        //}
    }
}
