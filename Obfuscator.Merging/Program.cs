using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.IO;

namespace Merger
{
    class Program
    {

        static void Main(string[] args)
        {
            string directory = @"..\..\TestAssemblies\Gendarme\";
            string output = @"..\..\output\";
            
            IList<string> toMerge = new List<string>(), toCopy = new List<string>();

            foreach(var file in Directory.EnumerateFiles(directory, "Gendarme.Rules.*",SearchOption.TopDirectoryOnly))
            {
                FileInfo fi = new FileInfo(file);
                toMerge.Add(fi.Name);
            }

            
            //toMerge = new List<string>() { "gendarme.exe" };
            //toCopy = new List<string>() { "gendarme.exe" };
            //toCopy = new List<string>() { "Castle.Windsor.dll", "Castle.Core.dll" };
           // toMerge = new List<string>() { "monolinker.exe", "Mono.Cecil.dll" , "Mono.Cecil.Rocks.dll" };
            //toMerge = new List<string>() { "JanuszLembicz.PW.UI.Forms.exe", "JanuszLembicz.PW.BLC.dll", /*"JanuszLembicz.PW.Common.dll",*/ "JanuszLembicz.PW.UI.Form.Utils.dll" };
            //toCopy = new List<string>() { "Mono.Cecil.dll", "Mono.Cecil.Rocks.dll","monolinker.exe", "Lucene.Net.dll" };

            IList<AssemblyDefinition> assembliesToMerge = new List<AssemblyDefinition>();
            IList<AssemblyDefinition> assembliesToCopy = new List<AssemblyDefinition>();

            AssemblyResolver resolver = new AssemblyResolver();

            foreach(var dir in resolver.GetSearchDirectories())
            {
                resolver.RemoveSearchDirectory(dir);
            }
            resolver.AddSearchDirectory(directory);

            var parameters = new ReaderParameters()
            {
                AssemblyResolver = resolver,
            };


            foreach(var assembly in toMerge)
            {
                assembliesToMerge.Add(AssemblyDefinition.ReadAssembly(directory + assembly, parameters));
            }

            foreach(var assembly in toCopy)
            {
                assembliesToCopy.Add(AssemblyDefinition.ReadAssembly(directory + assembly, parameters));
            }

            MergeAssemblies merger = new MergeAssemblies();

            Console.WriteLine("Merging");
            try
            {
                AssemblyDefinition merged = merger.MergeAssemblies(assembliesToMerge);
                if(merged != null)
                    merged.Write(output + merged.Name.Name + (merged.MainModule.Kind == ModuleKind.Dll ? ".dll" : ".exe"));
            }
            catch(OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();

            Console.WriteLine("Copying");
            foreach(var assembly in assembliesToCopy)
            {
                AssemblyDefinition copy = merger.CopyAssembly(assembly);
                if(copy != null)
                    copy.Write(output + copy.Name.Name + (copy.MainModule.Kind == ModuleKind.Dll ? ".dll" : ".exe"));
            }

            Console.ReadKey();

        }
    }
}





