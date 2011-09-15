using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Configuration.COM;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Obfuscator.Utils;

namespace Obfuscator.Configuration
{

    public abstract class InputConfiguration : IFilter
    { 
        private AssemblyResolver _resolver;
        private ReaderParameters _readerParameters;

        private List<Assembly> _assemblies = new List<Assembly>();

        public List<Assembly> Assemblies
        {
            get
            {
                return _assemblies;
            }
        }

        public List<string> SearchDirectories
        {
            get
            {
                return _resolver.GetSearchDirectories().ToList();
            }
        }

        protected List<Member> Skip
        {
            get;
            protected abstract set;
        }

        protected List<Method> KeepStrings
        {
            get;
            protected abstract set;
        }

        protected List<Assembly> KeepNamespaces
        {
            get;
            protected abstract set;
        }

        public InputConfiguration()
        {            
            _resolver = new AssemblyResolver();            
            _readerParameters = new ReaderParameters();            
        }

        bool IFilter.ShouldSkip(IMetadataTokenProvider token)
        {
            // TODO improve
            return Skip.Count(m => m.MemberDefinition.MetadataToken == token.MetadataToken) == 1;
        }

        public void AddAssembly(string name)
        {
            _assemblies.Add(ResolveAssembly(name));
        }

        public void AddSearchDirectory(string directory)
        {
            _resolver.AddSearchDirectory(directory);            
        }

        public Assembly ResolveAssembly(string name)
        {
            return new Assembly(Resolve(name));
        }

        protected static IList<COM.Type> Types(Assembly assembly)
        {            
            AssemblyDefinition resoledAssembly = assembly.AssemblyDefinition;
            var types = resoledAssembly.MainModule.GetAllTypes();
            var result = types.Where(t => t.Name != "<Module>").Select(t => new COM.Type(t)).ToList();
            return result;
        }

        protected static IList<Method> Methods(COM.Type type)
        {
            var typeDefinition = GetTypeDefinition(type);
            var result = typeDefinition.Methods.Select(m => new Method(m)).ToList();
            return result;
        }

        protected static IList<Property> Properties(COM.Type type)
        {
            var typeDefinition = GetTypeDefinition(type);
            var result = typeDefinition.Properties.Select(p => new Property(p)).ToList();
            return result;
        }

        protected static IList<Field> Fields(COM.Type type)
        {
            var typeDefinition = GetTypeDefinition(type);
            var result = typeDefinition.Fields.Select(f => new Field(f)).ToList();
            return result;
        }

        protected static IList<Event> Events(COM.Type type)
        {
            var typeDefinition = GetTypeDefinition(type);
            var result = typeDefinition.Events.Select(e => new Event(e)).ToList();
            return result;
        }

        private static TypeDefinition GetTypeDefinition(COM.Type type)
        {
            AssemblyDefinition assembly = type.Assembly.AssemblyDefinition;
            var typeDefinition = assembly.MainModule.GetType(type.FullName);
            return typeDefinition;
        }
        
        private AssemblyDefinition Resolve(string name)
        {
            if (File.Exists(name))
            {
                AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(name, _readerParameters);
                _resolver.CacheAssembly(assemblyDefinition);
                return assemblyDefinition;
            }
            return _resolver.Resolve(new AssemblyNameReference(name, new Version()), _readerParameters);
        }
    }
}

