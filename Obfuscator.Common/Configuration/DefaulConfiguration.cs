using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Configuration.COM;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Obfuscator.Configuration
{
    public sealed class DefaultConfiguration : InputConfiguration
    {
        private List<Assembly> _assemblies = new List<Assembly>();

        public List<Assembly> Assemblies
        {
            get
            {
                return _assemblies;
            }
        }

        private HashSet<Member> EntryPoints
        {
            get;
            set;
        }

        private Dictionary<Method, int> InvokeByName
        {
            get;
            set;
        }

        private List<Method> PreserveStrings
        {
            get;
            set;
        }

        private List<Assembly> KeepNamespaces
        {
            get;
            set;
        }

        public void AddAssembly(string name)
        {
            _assemblies.Add(ResolveAssembly(name));
        }

        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return _assemblies;
        }

        protected override bool IsEntryPoint(Member member)
        {
            if (EntryPoints == null)
                SetEntryPoints();
            return EntryPoints.Contains(member);
        }

        //protected override bool ShouldPreserveStrings(Method method)
        //{
        //    return false;
        //}

        protected override bool ShouldKeepNamespacess(Assembly assembly)
        {
            return false;
        }

        protected override bool InvokesByName(Method method, out int paramIndex)
        {
            if (InvokeByName == null)
                SetInvokeByName();
            var x = InvokeByName.ContainsKey(method);
            if (InvokeByName.TryGetValue(method, out paramIndex))
                return true;
            return false;
        }

        private void SetEntryPoints()
        {
            var skipList = new HashSet<Member>();
            foreach (var assembly in Assemblies)
            {
                var types = Types(assembly);
                foreach (var type in types)
                {
                    //skipList.AddRange(Methods(type).Where(m => m.Name == "MethodA"));
                }
            }
            EntryPoints = skipList;
        }

        private void SetInvokeByName()
        {
            InvokeByName = new Dictionary<Method, int>();

            // TODO: move to input configuration?
            ModuleDefinition corlib = ModuleDefinition.ReadModule(typeof(object).Module.FullyQualifiedName);
            var typeType = corlib.GetType("System.Type");
            var methods = typeType.GetMethods().Where(m => m.HasParameters && m.Parameters[0].ParameterType.FullName == "System.String");

            foreach (var method in methods)
                InvokeByName.Add(new Method(method), 1);

            var assemblyType = corlib.GetType("System.Reflection.Assembly");
            var assemblyMethods = assemblyType.GetMethods().Where(m => m.HasParameters && m.Parameters[0].ParameterType.FullName == "System.String");


            foreach (var method in assemblyMethods)
                InvokeByName.Add(new Method(method), 1);


            var resourcesType = Types(ResolveAssembly("mscorlib")).First(t => t.FullName == "System.Resources.ResourceManager");
            var resourceManagerMethods = Methods(resourcesType).Where(m => m.IsConstructor && m.ParametersCount > 0 && m.Parameters.ElementAt(0).FullName == "System.String");
            
            foreach (var method in resourceManagerMethods)
                InvokeByName.Add(method, 1);                                    
        }        
    }
}
