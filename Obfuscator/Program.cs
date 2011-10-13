using CommandLine;
using Obfuscator.Configuration;
using Obfuscator.Steps;
using Obfuscator.Configuration.COM;
using System.Collections.Generic;
using Obfuscator.Renaming;
using Obfuscator.Common.Configuration;
using Obfuscator.Common;
using Obfuscator.Renaming.Steps;
using Obfuscator.Merger;

namespace Obfuscator
{
    public class Options
    {        
        [Option("r", "reflection", Required=false)]
        public bool InsertRenameMaps = false;

        [Option("c", "ctscompliance", Required=false)]
        public bool CTSCompliance = false;

        [Option("n", "namespaces", Required=false)]
        public bool KeepNamespaces = false;

        [Option("m", "merge", Required = false)]
        public bool Merge = false;

        [OptionArray("a", "assembly", Required = true, HelpText = "Input assembly name")]
        public string[] AssemblyNames;

        [Option("i", "inputDir", Required = true, HelpText = "Directory containing input assemblies")]
        public string InputDir;

        [Option("o", "outputDir", Required = true, HelpText = "Output directory")]
        public string OutputDir;

        //[OptionArray("r", "referencingAssembly", Required = false, HelpText = "Referencing assembly names")]
        //public string[] ReferencingAssemblyNames;
    }

    public class Program
    {      
        static void Main(string[] args)
        {
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options))
            {
                ObfuscationContext context = new ObfuscationContext();
                context.AddSearchDirectory(options.InputDir);
                context.OutputDirectory = options.OutputDir;

                foreach (var assembly in options.AssemblyNames)
                {
                    context.AddAssembly(assembly);
                }
                                
                Pipeline p = GetPipeline(options);
               
                p.Process(context);              
                System.Console.ReadKey();
            }            
        }

        private static RenamingOptions GetRenamignOptions(Options options)
        {
            RenamingOptions result = RenamingOptions.Default;
            if(options.CTSCompliance)
                result |= RenamingOptions.CTSCompliance;
            if (options.KeepNamespaces)
                result |= RenamingOptions.KeepNamespaces;
            if (options.InsertRenameMaps)
                result |= RenamingOptions.Reflection;            
            return result;
        }

        private static ReflectionOptions GetReflectionOptions(Options options)
        {            
            return ReflectionOptions.Types | ReflectionOptions.Methods | ReflectionOptions.Fields | ReflectionOptions.Properties | ReflectionOptions.NestedTypes;                                            
        }

        static Pipeline GetPipeline(Options options)
        {
            Pipeline p = new Pipeline();
            if (options.Merge)
                p.AppendStep(new MergeAssembliesStep());
            p.AppendStep(new RenameStep(GetRenamignOptions(options), GetReflectionOptions(options)));
            p.AppendStep(new OutputStep());
            return p;
        }
    }
}


