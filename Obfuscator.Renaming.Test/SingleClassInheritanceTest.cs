using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using Mono.Cecil;

namespace Obfuscator.Renaming.Test
{
    [TestClass]
    public class SingleClassInheritanceTest
    {
        //private TypeTree _assemblyMap;

        //private const string NAMESPACE = "Obfuscator.Test";

        //private const string testAssemblyName1 = NAMESPACE + ".Library1";
        //private const string testAssemblyName2 = NAMESPACE + ".Library2";

        //private AssemblyDefinition assembly1;
        //private AssemblyDefinition assembly2;

        //TypeDefinition singleClassInheritanceClass;
        //TypeDefinition singleClassInheritanceBaseClass;

        //[TestInitialize]
        //public void SetUp()
        //{
        //    assembly1 = AssemblyDefinition.ReadAssembly(testAssemblyName1);
        //    assembly2 = AssemblyDefinition.ReadAssembly(testAssemblyName2);

        //    singleClassInheritanceClass = assembly1.MainModule.GetType(NAMESPACE, "SingleClassInheritanceClass");
        //    singleClassInheritanceBaseClass = assembly1.MainModule.GetType(NAMESPACE, "SingleClassInheritanceBaseClass");

        //    var assemblies = new List<AssemblyDefinition>() {
        //        assembly1,
        //        assembly2
        //    };

        //    _assemblyMap = new TypeTree(assemblies);
        //}


        //public static MethodDefinition GetSingleMethodByName(TypeDefinition type, string name)
        //{
        //    return type.Methods.Single(m => m.Name == name);
        //}

        //public void ExpectEqual(TypeDefinition type, string name, IList<MethodDefinition> expected)
        //{
        //    var actual = _assemblyMap.GetRelatedMethods(GetSingleMethodByName(type, name));
        //    CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        //}

        //public void ExpectNothing(TypeDefinition type, string name)
        //{
        //    ExpectEqual(type, name, new List<MethodDefinition>());
        //}

        //[TestMethod]
        //public void TestInstanceMethods()
        //{
        //    ExpectNothing(singleClassInheritanceClass, "ImplicitNewInstanceMethod");
        //    ExpectNothing(singleClassInheritanceClass, "ExplicitNewInstanceMethod");

        //}

        //// TODO maybe it's enough to fill the override table with methods that already ovverride given method and then rename them to whatever names keeping the polymorphism features

        //[TestMethod]
        //public void TestVirtualMethods()
        //{
        //    ExpectNothing(singleClassInheritanceClass, "ImplicitNewVirtualMethod");
        //    ExpectNothing(singleClassInheritanceClass, "ExplicitNewVirtualMethod");

        //    ExpectEqual(singleClassInheritanceClass, "ExplicitOverrideVirtualMethod", new List<MethodDefinition>()
        //    {
        //        GetSingleMethodByName(singleClassInheritanceBaseClass,"ExplicitOverrideVirtualMethod")
        //    });

        //    ExpectNothing(singleClassInheritanceClass, "NewSlotVirtualMethod");
        //}
    }
}
