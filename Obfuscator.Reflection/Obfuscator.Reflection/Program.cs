using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Obfuscator.Utils;
using Mono.Cecil;
using CommandLine;
using System.IO;

namespace Reflection
{
    public class Options
    {
        [Option("p", "path", Required = false, HelpText = "Input assembly path")]
        public string AssemblyPath;


        [Option("a", "assembly", Required = true, HelpText = "Input assembly name")]
        public string Assembly;
    }

    class Program
    {
        public const string TEST_LIBRARIES = @"D:\Magisterka-nowy\Obfuscator\Obfuscator.Reflection\TestLibraries\bin\";
        public const string OUTPUT = @"D:\Magisterka-nowy\Obfuscator\Obfuscator.Reflection\TestLibraries\output\";

        static void Main(string[] args)
        {       
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options))
            {
                AssemblyVisitor visitor = new AssemblyVisitor();
                DiscoveringReflection v = new DiscoveringReflection();

                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(TEST_LIBRARIES + options.Assembly);

                visitor.ConductVisit(assembly, v);

                assembly.Write(GetAssemblyFileName(assembly, OUTPUT));

                Console.ReadKey();
            }

        }

        static string GetAssemblyFileName(AssemblyDefinition assembly, string directory)
        {
            string file = assembly.Name.Name + (assembly.MainModule.Kind == ModuleKind.Dll ? ".dll" : ".exe");
            return Path.Combine(directory, file);
        }
    }
}
