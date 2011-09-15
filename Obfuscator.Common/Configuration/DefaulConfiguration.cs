using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Configuration.COM;

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

        private List<Member> EntryPoints
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

        protected override bool ShouldPreserveStrings(Method method)
        {
            return false;
        }

        protected override bool ShouldKeepNamespacess(Assembly assembly)
        {
            return false;
        }

        private void SetEntryPoints()
        {
            var skipList = new List<Member>();
            foreach (var assembly in Assemblies)
            {
                var entryTypes =
                    from t in Types(assembly)
                    where Methods(t).Any(m => m.Name == "Main")
                    select t;
                skipList.AddRange(entryTypes);
            }
            EntryPoints = skipList;

        }
    }
}
