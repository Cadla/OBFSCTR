//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using CommandLine;
//using Mono.Cecil;
//using Obfuscator.Utils;
//using System.IO;

//namespace Obfuscator.OverridesResolver
//{
//    public class Options
//    {
//        [Option("p", "path", Required = false, HelpText = "Input assembly path")]
//        public string AssemblyPath;


//        [Option("a", "assembly", Required = true, HelpText = "Input assembly name")]
//        public string AssemblyName;
//    }

//    public class Program
//    {
//        public const string TEST_LIBRARIES = @"D:\Magisterka-nowy\Obfuscator\TestLibraries\bin\";
//        public const string OUTPUT = @"D:\Magisterka-nowy\Obfuscator\TestLibraries\output\";

//        static void Main(string[] args)
//        {
//            var options = new Options();
//            ICommandLineParser parser = new CommandLineParser();
//            if (parser.ParseArguments(args, options))
//            {
//                AssemblyResolver resolver = new AssemblyResolver();

//                foreach (var dir in resolver.GetSearchDirectories())
//                {
//                    resolver.RemoveSearchDirectory(dir);
//                }
//                resolver.AddSearchDirectory(TEST_LIBRARIES);
//                resolver.AddSearchDirectory(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client");

//                var parameters = new ReaderParameters()
//                {
//                    AssemblyResolver = resolver,
//                };
                
//                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(TEST_LIBRARIES + options.AssemblyName, parameters);

//                Visitor testVisitor = new Visitor();
//                AssemblyVisitor visitor = new AssemblyVisitor(true);

//                visitor.ConductVisit(assembly, testVisitor);

//                testVisitor.printDictionary();

//                assembly.Name.Version = new System.Version(666, 666);
//                assembly.Name.Name = assembly.Name.Name + "_modified";
//                var stream = new FileStream(OUTPUT + assembly.Name.Name + (assembly.MainModule.Kind == ModuleKind.Dll ? ".dll" : ".exe"), FileMode.Create);
//                assembly.Write(stream);

//                System.Console.ReadKey();
//            }
//        }
//    }
//}
