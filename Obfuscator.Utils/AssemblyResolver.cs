using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Collections;
using System.IO;

namespace Obfuscator.Utils
{
    public class AssemblyResolver : BaseAssemblyResolver
    {
        IDictionary _assemblies;

        public IDictionary AssemblyCache
        {
            get { return _assemblies; }
        }

        public AssemblyResolver()
            : this(new Hashtable())
        {
        }

        public AssemblyResolver(IDictionary assembly_cache)
        {
            _assemblies = assembly_cache;
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            AssemblyDefinition asm = (AssemblyDefinition)_assemblies[name.Name];
            if (asm == null)
            {
                asm = base.Resolve(name, parameters);
                _assemblies[name.Name] = asm;
            }
            return asm;
        }

        public void CacheAssembly(AssemblyDefinition assembly)
        {
            _assemblies[assembly.Name.Name] = assembly;
            base.AddSearchDirectory(Path.GetDirectoryName(assembly.MainModule.FullyQualifiedName));
        }
    }
}
