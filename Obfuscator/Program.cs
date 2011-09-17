using CommandLine;
using Obfuscator.Configuration;
using Obfuscator.Steps;
using Obfuscator.Steps.Reflection;
using Obfuscator.Steps.Renaming;

namespace Obfuscator
{
    public class Options
    {        
        [Option("p", "path", Required=false, HelpText="Input assembly path")]
        public string AssemblyPath;


        [OptionArray("a", "assembly", Required = true, HelpText = "Input assembly name")]
        public string[] AssemblyNames;
    }

    public class Program
    {
        public const string TEST_LIBRARIES = @"D:\Magisterka-nowy\Obfuscator\TestLibraries\bin\";
        public const string NOT_NECESSARY = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client";
        public const string OUTPUT = @"D:\Magisterka-nowy\Obfuscator\output\";

        static void Main(string[] args)
        {
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options))
            {
                DefaultConfiguration configuration = new DefaultConfiguration();

                configuration.AddSearchDirectory(TEST_LIBRARIES);
                configuration.AddSearchDirectory(NOT_NECESSARY);

                
                foreach (var assembly in options.AssemblyNames)
                {
                    configuration.AddAssembly(assembly);
                }

                ObfuscationContext context = new ObfuscationContext(configuration, ObfuscationOptions.CLSCompliance);
                
                context.OutputDirectory = OUTPUT;
                
                Pipeline p = GetStandardPipeline();                
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
            p.AppendStep(new ReplaceMemberNameStringsStep());
            p.AppendStep(new RenameStep());
            p.AppendStep(new OutputStep());
            return p;
        }
    }
}


