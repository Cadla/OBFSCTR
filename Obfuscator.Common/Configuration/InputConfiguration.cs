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

        public List<string> SearchDirectories
        {
            get
            {
                return _resolver.GetSearchDirectories().ToList();
            }
        }

        public InputConfiguration()
        {
            _resolver = new AssemblyResolver();
            _readerParameters = new ReaderParameters() { ReadSymbols = true };
        }

        protected abstract bool IsEntryPoint(Member member);
        protected abstract bool InvokedByName(Method method, out int nameIndex, out int typeInstanceIndex);
        protected abstract bool AccessedByName(COM.Type type, out int nameIndex);
        //protected abstract bool ShouldPreserveStrings(Method method);
        protected abstract bool ShouldKeepNamespacess(Assembly assembly);

        protected abstract IEnumerable<Assembly> GetAssemblies();        

        public void AddSearchDirectory(string directory)
        {
            _resolver.AddSearchDirectory(directory);
        }

        internal IEnumerable<AssemblyDefinition> GetAssemblyDefinitions()
        {
            return GetAssemblies().Select(a => a.AssemblyDefinition);
        }

        bool IFilter.AccessedByName(TypeReference type, out int nameIndex)
        {
            return AccessedByName(new COM.Type(type), out nameIndex);
        }

        bool IFilter.InvokedByName(MethodReference method, out int nameIndex, out int typeInstanceIndex)
        {
            return InvokedByName(new Method(method), out nameIndex, out typeInstanceIndex);
        }
        
        bool IFilter.ShouldKeepNamespaces(AssemblyDefinition assembly)
        {
            return ShouldKeepNamespacess(new Assembly(assembly));
        }

        bool IFilter.ShouldSkip(IMemberDefinition member)
        {
            return IsEntryPoint(new Member(member));
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

        protected Assembly ResolveAssembly(string name)
        {
            return new Assembly(Resolve(name));
        }

        public AssemblyDefinition Resolve(string name)
        {
            //if (File.Exists(name))
            //{
            //    AssemblyDefinition assemblyDefinition;
            //    if(name.Contains("mscorlib"))
            //        assemblyDefinition = AssemblyDefinition.ReadAssembly(name, new ReaderParameters() {ReadSymbols = false});
            //    else
            //        assemblyDefinition = AssemblyDefinition.ReadAssembly(name, _readerParameters);

            //    if(_resolver.Resolve(assemblyDefinition.Name, _readerParameters))

            //    _resolver.CacheAssembly(assemblyDefinition);
            //    return assemblyDefinition;
            //}
            
            if(name == "mscorlib")
                return _resolver.Resolve(new AssemblyNameReference(name, new Version()), new ReaderParameters{ReadSymbols = false});
            return _resolver.Resolve(new AssemblyNameReference(name, new Version()), _readerParameters);
        }
    }
}

