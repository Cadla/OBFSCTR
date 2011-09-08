using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.OverridesResolver;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;
using System.Collections;

namespace Obfuscator.Renaming
{
    public class ObfuscationContext
    {
        Pipeline _pipeline;
        string _outputDirectory;
        bool _linkSymbols;

        AssemblyResolver _resolver;

        ReaderParameters _readerParameters;
        ISymbolReaderProvider _symbolReaderProvider;
        ISymbolWriterProvider _symbolWriterProvider;
       
        public Pipeline Pipeline
        {
            get { return _pipeline; }
        }

        public string OutputDirectory
        {
            get { return _outputDirectory; }
            set { _outputDirectory = value; }
        }

        public bool LinkSymbols
        {
            get { return _linkSymbols; }
            set { _linkSymbols = value; }
        }

        public AssemblyResolver Resolver
        {
            get { return _resolver; }
        }

        public ISymbolReaderProvider SymbolReaderProvider
        {
            get { return _symbolReaderProvider; }
            set { _symbolReaderProvider = value; }
        }

        public ISymbolWriterProvider SymbolWriterProvider
        {
            get { return _symbolWriterProvider; }
            set { _symbolWriterProvider = value; }
        }

        public Dictionary<AssemblyNameDefinition, Renamer> Renamers
        {
            get;
            set;
        }

        public ObfuscationContext(Pipeline pipeline)
            : this(pipeline, new AssemblyResolver())
        {
        }

        public ObfuscationContext(Pipeline pipeline, AssemblyResolver resolver)
        {
            _pipeline = pipeline;
            _resolver = resolver;
            _readerParameters = new ReaderParameters
            {
                AssemblyResolver = _resolver,
            };
        }

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

        public AssemblyDefinition Resolve(string name)
        {
            if (File.Exists(name))
            {
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(name, _readerParameters);
                _resolver.CacheAssembly(assembly);
                return assembly;
            }

            return Resolve(new AssemblyNameReference(name, new Version()));
        }

        public AssemblyDefinition Resolve(IMetadataScope scope)
        {
            AssemblyNameReference reference = GetReference(scope);

            AssemblyDefinition assembly = _resolver.Resolve(reference, _readerParameters);

            return assembly;
        }

        //public void SafeReadSymbols (AssemblyDefinition assembly)
        //{
        //    if (!_linkSymbols)
        //        return;

        //    if (assembly.MainModule.HasSymbols)
        //        return;

        //    try {
        //        if (_symbolReaderProvider != null) {
        //            var symbolReader = _symbolReaderProvider.GetSymbolReader (
        //                assembly.MainModule,
        //                assembly.MainModule.FullyQualifiedName);

        //        //	_annotations.AddSymbolReader (assembly, symbolReader);
        //            assembly.MainModule.ReadSymbols (symbolReader);
        //        } else
        //            assembly.MainModule.ReadSymbols ();
        //    } catch {}
        //}

        static AssemblyNameReference GetReference(IMetadataScope scope)
        {
            AssemblyNameReference reference;
            if (scope is ModuleDefinition)
            {
                AssemblyDefinition asm = ((ModuleDefinition)scope).Assembly;
                reference = asm.Name;
            }
            else
                reference = (AssemblyNameReference)scope;

            return reference;
        }

        static bool IsCore(AssemblyNameReference name)
        {
            switch (name.Name)
            {
                case "mscorlib":
                case "Accessibility":
                case "Mono.Security":
                    return true;
                default:
                    return name.Name.StartsWith("System")
                        || name.Name.StartsWith("Microsoft");
            }
        }

        public AssemblyDefinition[] GetAssemblies()
        {
            IDictionary cache = _resolver.AssemblyCache;
            AssemblyDefinition[] asms = new AssemblyDefinition[cache.Count];
            cache.Values.CopyTo(asms, 0);
            return asms;
        }
    }
}
