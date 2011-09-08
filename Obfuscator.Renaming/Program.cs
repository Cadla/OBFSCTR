using System.Linq;
using Mono.Cecil;
using System.Text.RegularExpressions;
using Obfuscator.Utils;
using CommandLine;
using System.IO;
using Obfuscator.Renaming.Visitors;
using Obfuscator.Renaming.Steps;

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

                var parameters = new ReaderParameters()
                {
                    AssemblyResolver = resolver,
                    ReadSymbols = true
                };

                Pipeline p = GetStandardPipeline();                
                ObfuscationContext context = new ObfuscationContext(p, resolver);

                context.OutputDirectory = OUTPUT;

                context.Resolver.AddSearchDirectory(TEST_LIBRARIES);
                context.Resolver.AddSearchDirectory(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client");

                context.Resolve(TEST_LIBRARIES + options.AssemblyName);
                context.Resolve(TEST_LIBRARIES + "ConsoleApplication1.exe");                

                p.Process(context);              

                System.Console.ReadKey();
            }
        }

        static Pipeline GetStandardPipeline()
        {
            Pipeline p = new Pipeline();
            p.AppendStep(new FillOverrideTables());
            p.AppendStep(new BuildRenameMapStep());
       //     p.AppendStep(new FixVirtualMethodsNames());
            p.AppendStep(new FixReferencesStep());
            p.AppendStep(new RenameStep());
            p.AppendStep(new OutputStep());
            return p;
        }
    }
}


