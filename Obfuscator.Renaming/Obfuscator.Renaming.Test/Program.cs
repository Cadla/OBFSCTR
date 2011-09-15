//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Obfuscator.Configuration;
//using Obfuscator.Configuration.DOM;
//using CommandLine;

//namespace ConsoleApplication1
//{
//    public class Options
//    {
//        [Option("p", "path", Required = false, HelpText = "Input assembly path")]
//        public string AssemblyPath;


//        [OptionArray("a", "assembly", Required = true, HelpText = "Input assembly name")]
//        public string[] AssemblyNames;
//    }

//    

//    class Program
//    {
//        public const string TEST_LIBRARIES = @"D:\Magisterka-nowy\Obfuscator\Obfuscator.Renaming\TestLibraries\bin\";
//        public const string OUTPUT = @"D:\Magisterka-nowy\Obfuscator\TestLibraries\output\";

//        static void Main(string[] args)
//        {
//            var options = new Options();
//            ICommandLineParser parser = new CommandLineParser();
//            if (parser.ParseArguments(args, options))
//            {
//                MyConfiguration configuraiton = new MyConfiguration();

//                configuraiton.AddSearchDirectory(TEST_LIBRARIES);
//                foreach (var assembly in options.AssemblyNames)
//                {
//                    configuraiton.AddAssembly(assembly);
//                }
                
//                var skipMembers = configuraiton.Skip();
//            }

//        }
//    }
//}
