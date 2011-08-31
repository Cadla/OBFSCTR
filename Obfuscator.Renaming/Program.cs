using System.Linq;
using Mono.Cecil;
using System.Text.RegularExpressions;
using Obfuscator.Utils;

namespace Obfuscator.Renaming
{
    public class Program
    {
        static void Main(string[] args)
        {
            //string assemblyPath = args[0];

            string assemblyPath = "Obfuscator.Test.Library1.dll";

            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
       
            RenameVisitor testVisitor = new RenameVisitor();
            AssemblyVisitor visitor = new AssemblyVisitor(false);

            visitor.ConductVisit(assembly, testVisitor);

            testVisitor.PringMap();


            assembly.Name.Version = new System.Version(666, 666);
            assembly.Name.Name = "Obfuscator.Test.Library_modified";
            assembly.Write("Obfuscator.Test.Library_modified.dll");

            System.Console.ReadKey();
        }
    }
}
