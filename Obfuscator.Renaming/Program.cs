using System.Linq;
using Mono.Cecil;
using System.Text.RegularExpressions;
using Obfuscator.Utils;
using CommandLine;
using System.IO;

namespace Obfuscator.Renaming
{
    public class Options
    {
        [Option("p", "path", Required=false, HelpText="Input assembly path")]
        public string AssemblyPath;


        [Option("a", "assembly", Required = true, HelpText = "Input assembly name")]
        public string AssemblyName;
    }

    public class Program
    {
        public const string TEST_LIBRARIES = @"D:\Magisterka-nowy\Obfuscator\TestLibraries\bin\";
        public const string OUTPUT = @"D:\Magisterka-nowy\Obfuscator\TestLibraries\output\";

        static void Main(string[] args)
        {
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options))
            {
                OverridesResolver.AssemblyResolver resolver = new OverridesResolver.AssemblyResolver();

                foreach (var dir in resolver.GetSearchDirectories())
                {
                    resolver.RemoveSearchDirectory(dir);
                }
                resolver.AddSearchDirectory(TEST_LIBRARIES);
                resolver.AddSearchDirectory(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client");

                var parameters = new ReaderParameters()
                {
                    AssemblyResolver = resolver,
                    ReadSymbols = true
                };

                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(TEST_LIBRARIES + options.AssemblyName, parameters); 
                //AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(TEST_LIBRARIES + options.AssemblyName);

                RenameVisitor testVisitor = new RenameVisitor();
                OverridesResolver.Visitor overridesVisitor = new OverridesResolver.Visitor();

                AssemblyVisitor visitor = new AssemblyVisitor(true);

                visitor.ConductVisit(assembly, overridesVisitor);

                overridesVisitor.printDictionary();                

                visitor.ConductVisit(assembly, testVisitor);                

                FixVirtualMethodsNaminVisitor fixVisitor = new FixVirtualMethodsNaminVisitor(testVisitor._scope, testVisitor._renameMap);

                visitor.ConductVisit(assembly, fixVisitor);

                testVisitor.PringMap();

                assembly.Name.Version = new System.Version(666, 666);
                assembly.Name.Name = assembly.Name.Name+"_modified";
                var stream = new FileStream(OUTPUT + assembly.Name.Name + (assembly.MainModule.Kind == ModuleKind.Dll ? ".dll" : ".exe"), FileMode.Create);
                
                assembly.Write(stream, new WriterParameters() { WriteSymbols = true });

                System.Console.ReadKey();
            }
        }
    }
}
