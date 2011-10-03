using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;
using System.Collections;
using Obfuscator.Utils;

namespace Obfuscator.Common
{
    public class ObfuscationContext
    {
        private AssemblyResolver _resolver;
        private ReaderParameters _readerParameters;

        HashSet<AssemblyDefinition> _assemblies = new HashSet<AssemblyDefinition>();
        HashSet<AssemblyDefinition> _satelites = new HashSet<AssemblyDefinition>();

        public bool Symbols { get; set; }

        public IMemberFilter Filter
        {
            get;
            set;
        }
        
        public string OutputDirectory
        {
            get;
            set;
        }

        public ObfuscationContext(bool symbols = false)        
        {           
             _resolver = new AssemblyResolver();
             Symbols = symbols;
            _readerParameters = new ReaderParameters() { ReadSymbols = Symbols };
            Filter = new DefaultFilter();
        }

        public ObfuscationContext(ObfuscationContext context)
        {
            _resolver = context._resolver;
            _readerParameters = context._readerParameters;
            _assemblies = context._assemblies;
            _satelites = context._satelites;
            Symbols = context.Symbols;
            Filter = context.Filter;
            OutputDirectory = context.OutputDirectory;

        }

        public List<string> SearchDirectories
        {
            get
            {
                return _resolver.GetSearchDirectories().ToList();
            }
        }
        
        public void AddSearchDirectory(string directory)
        {
            _resolver.AddSearchDirectory(directory);
        }

        public void AddAssembly(string name)
        {
            AddAssembly(Resolve(name));
        }

        public void AddAssembly(AssemblyDefinition assembly)
        {
            _assemblies.Add(assembly);
        }

        public void RemoveAssembly(string name)
        {
            RemoveAssembly(Resolve(name));
        }

        public void RemoveAssembly(AssemblyDefinition assembly)
        {
            _assemblies.Remove(assembly);
        }

        public AssemblyDefinition Resolve(string name)
        {
            IMetadataScope scope;
            if (File.Exists(name))
                scope = AssemblyDefinition.ReadAssembly(name, _readerParameters).Name;
            else
                scope = new AssemblyNameReference(name, new Version());
            return Resolve(scope);
        }

        public AssemblyDefinition Resolve(IMetadataScope scope)
        {
            AssemblyNameReference reference = GetReference(scope);
            return _resolver.Resolve(reference, _readerParameters);
        }

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


        public AssemblyDefinition[] GetAssemblies()
        {
            //IDictionary cache = _resolver.AssemblyCache;
            //AssemblyDefinition[] asms = new AssemblyDefinition[cache.Count];
            //cache.Values.CopyTo(asms, 0);
            AssemblyDefinition[] asms = new AssemblyDefinition[_assemblies.Count];
            _assemblies.CopyTo(asms, 0);
            return asms;
        }
    }
}
