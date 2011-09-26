﻿using CommandLine;
using Obfuscator.Configuration;
using Obfuscator.Steps;
using Obfuscator.Steps.Reflection;
using Obfuscator.Steps.Renaming;
using Obfuscator.Configuration.COM;
using System.Collections.Generic;
using Mono.Cecil;


namespace Obfuscator
{
    public class Options
    {        
        [Option("m", "renameMaps", Required=false)]
        public bool InsertRenameMaps = false;

        [Option("c", "ctscompliance", Required=false)]
        public bool CTSCompliance = false;

        [Option("n", "namespaces", Required=false)]
        public bool KeepNamespaces = false;

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
                DefaultConfiguration configuration = new DefaultConfiguration();

                configuration.AddSearchDirectory(options.InputDir);
                                       
                foreach (var assembly in options.AssemblyNames)
                {
                    configuration.AddAssembly(assembly);
                }

                ObfuscationContext context = new ObfuscationContext(configuration, GetObfuscationOptions(options));
                
                context.OutputDirectory = options.OutputDir;
                
                Pipeline p = GetStandardPipeline();
                if (options.InsertRenameMaps)
                    p.AddStepAfter(typeof(BuildRenameMapStep), new ReplaceReflectionMethodsParameters());                    

                p.Process(context);              
                System.Console.ReadKey();
            }
            
        }

        private static ObfuscationOptions GetObfuscationOptions(Options options)
        {
            ObfuscationOptions result = ObfuscationOptions.Default;
            if(options.CTSCompliance)
                result |= ObfuscationOptions.CTSCompliance;
            if (options.InsertRenameMaps)
                result &= ~ObfuscationOptions.CTSCompliance;
            if (options.KeepNamespaces)
                result |= ObfuscationOptions.KeepNamespaces;
            return result;
        }

        static Pipeline GetStandardPipeline()
        {
            Pipeline p = new Pipeline();            
            p.AppendStep(new FillOverrideTables());
            p.AppendStep(new BuildRenameMapStep());
            p.AppendStep(new SaveRenameMap());
            p.AppendStep(new RenameReferencesStep());
            p.AppendStep(new RenameDefinitionsStep());         
            p.AppendStep(new OutputStep());
            return p;
        }
    }
}


