using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;

namespace Obfuscator.Renaming.Test
{
    [TestClass]
    public class NoInheritanceTest
    {
        private const string NAMESPACE = "Obfuscator.Test";

        private const string testAssembly = NAMESPACE + ".NoInheritance";


        private AssemblyDefinition assembly;
        
        TypeDefinition singleClassInheritanceClass;        

        [TestInitialize]
        public void SetUp()
        {
            assembly = AssemblyDefinition.ReadAssembly(testAssembly);
            singleClassInheritanceClass = assembly.MainModule.GetType(NAMESPACE, "SingleClassInheritanceClass");

            var assemblies = new List<AssemblyDefinition>() {
                assembly
            };
            
        }


        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
