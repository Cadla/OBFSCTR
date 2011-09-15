using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;
using System.Collections;
using Obfuscator.Configuration;

namespace Obfuscator
{
    public class ObfuscationContext
    {

        public ObfuscationOptions Options
        {
            get;
            private set;
        }

        public InputConfiguration InputConfiguration
        {
            get;
            private set;
        }
        
        public string OutputDirectory
        {
            get;
            set;
        }

        public Dictionary<AssemblyNameDefinition,
            Dictionary<IMemberDefinition, string>> DefinitionsRenameMap
        {
            get;
            private set;
        }

        public Dictionary<AssemblyNameDefinition,
            Dictionary<Resource, string>> ResourcesRenameMap
        {
            get;
            private set;
        }

        public Dictionary<AssemblyNameDefinition,
            Dictionary<MemberReference, string>> ReferencesRenameMap
        {
            get;
            private set;
        }

        public ObfuscationContext(InputConfiguration configuraiton, ObfuscationOptions options)        
        {
            InputConfiguration = configuraiton;
            Options = options;

            DefinitionsRenameMap = new Dictionary<AssemblyNameDefinition, Dictionary<IMemberDefinition, string>>();
            ResourcesRenameMap = new Dictionary<AssemblyNameDefinition, Dictionary<Resource, string>>();
            ReferencesRenameMap = new Dictionary<AssemblyNameDefinition, Dictionary<MemberReference, string>>();
        }

        ////Pipeline _pipeline;
        //string _outputDirectory;
        //bool _linkSymbols;

        //InputConfiguration _configuration;

        ////     AssemblyResolver _resolver;
        //IList<AssemblyDefinition> _assemblies;

        //   ReaderParameters _readerParameters;
        //   ISymbolReaderProvider _symbolReaderProvider;
        //   ISymbolWriterProvider _symbolWriterProvider;

        //public Pipeline Pipeline
        //{
        //    get { return _pipeline; }
        //}


        //public bool LinkSymbols
        //{
        //    get { return _linkSymbols; }
        //    set { _linkSymbols = value; }
        //}

        //public AssemblyResolver Resolver
        //{
        //    get { return _resolver; }
        //}

        //public ISymbolReaderProvider SymbolReaderProvider
        //{
        //    get { return _symbolReaderProvider; }
        //    set { _symbolReaderProvider = value; }
        //}

        //public ISymbolWriterProvider SymbolWriterProvider
        //{
        //    get { return _symbolWriterProvider; }
        //    set { _symbolWriterProvider = value; }
        //}



        //public Dictionary<string, string> Namespaces
        //{
        //    get;
        //    set;
        //}



        //public ObfuscationContext(Pipeline pipeline, AssemblyResolver resolver)
        //{
        //    _pipeline = pipeline;
        //    //_resolver = resolver;
        //    //_readerParameters = new ReaderParameters
        //    //{
        //    //    AssemblyResolver = _resolver,
        //    //    ReadSymbols = true
        //    //};
        //    _assemblies = new List<AssemblyDefinition>();
        //    Namespaces = new Dictionary<string, string>();
        //}

        //public TypeDefinition GetType (string fullName)
        //{
        //    int pos = fullName.IndexOf (",");
        //    fullName = fullName.Replace ("+", "/");
        //    if (pos == -1) {
        //        foreach (AssemblyDefinition asm in GetAssemblies ()) {
        //            var type = asm.MainModule.GetType (fullName);
        //            if (type != null)
        //                return type;
        //        }

        //        return null;
        //    }

        //    string asmname = fullName.Substring (pos + 1);
        //    fullName = fullName.Substring (0, pos);
        //    AssemblyDefinition assembly = Resolve (AssemblyNameReference.Parse (asmname));
        //    return assembly.MainModule.GetType (fullName);
        //}


        //public AssemblyDefinition[] GetAssemblies()
        //{
        //    //IDictionary cache = _resolver.AssemblyCache;
        //    //AssemblyDefinition[] asms = new AssemblyDefinition[cache.Count];
        //    //cache.Values.CopyTo(asms, 0);
        //    //return asms;
        //    return _assemblies.ToArray();
        //}
    }
}
