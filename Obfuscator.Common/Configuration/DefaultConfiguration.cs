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
        private List<Assembly> _referencingAssemblies = new List<Assembly>();

        public List<Assembly> Assemblies
        {
            get
            {
                return _assemblies;
            }
        }

        public List<Assembly> ReferencingAssemblies
        {
            get
            {
                return _referencingAssemblies;
            }
        }


        private HashSet<Member> EntryPoints
        {
            get;
            set;
        }

        private Dictionary<COM.Type, int> AccessedByN
        {
            get;
            set;
        }

        private Dictionary<Method, int> InvokedByN
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

        public void AddReferencingAssembly(string name)
        {
            _assemblies.Add(ResolveAssembly(name));
            _referencingAssemblies.Add(ResolveAssembly(name));
        }


        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return _assemblies.Union(_referencingAssemblies);
        }

        protected override bool IsReferencingAssembly(Assembly assembly)
        {
            return _referencingAssemblies.Contains(assembly);
        }

        protected override bool IsEntryPoint(Member member)
        {
            if (EntryPoints == null)
                SetEntryPoints();

            if (EntryPoints.Contains(member))
                return true;
            else if (EntryPoints.Contains(member.DeclaringType))
                return true;

            return false;
        }

        protected override bool ShouldKeepNamespacess(Assembly assembly)
        {
            return false;
        }

        //protected override bool InvokedByName(Method method, out int nameIndex, out int typeInstanceIndex)
        //{
        //    if (InvokedByN == null)
        //        SetInvokeByName();
        //    typeInstanceIndex = -1;

        //    if (InvokedByN.TryGetValue(method, out nameIndex))
        //        return true;
        //    return false;
        //}

        //protected override bool AccessedByName(COM.Type type, out int nameIndex)
        //{
        //    throw new NotImplementedException();
        //}
        

        private void SetEntryPoints()
        {
            var skipList = new HashSet<Member>();
            foreach (var assembly in Assemblies)
            {
                var types = Types(assembly);
                foreach (var type in types)
                {
                    if (type.Name == "Settings" || type.FullName == "Gendarme.Framework.EngineDependencyAttribute" || type.Name == "IAssemblyRule")
                        skipList.Add(type);                        
                }
            }
            EntryPoints = skipList;
        }
     
   
    }
}
