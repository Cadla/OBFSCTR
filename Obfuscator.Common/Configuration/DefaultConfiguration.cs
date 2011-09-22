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
        public DefaultConfiguration()
        {
            SetInvokeByName();
        }

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

        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return _assemblies;
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

        protected override bool InvokedByName(Method method, out int nameIndex, out int typeInstanceIndex)
        {
            if (InvokedByN == null)
                SetInvokeByName();
            typeInstanceIndex = -1;

            if (InvokedByN.TryGetValue(method, out nameIndex))
                return true;
            return false;
        }

        protected override bool AccessedByName(COM.Type type, out int nameIndex)
        {
            throw new NotImplementedException();
        }
        

        private void SetEntryPoints()
        {
            var skipList = new HashSet<Member>();
            foreach (var assembly in Assemblies)
            {
                var types = Types(assembly);
                foreach (var type in types)
                {
                    //if (type.FullName == "ICSharpCode.ILSpy.Xaml.XamlResourceNodeFactory" ||
                    //   type.FullName == "ICSharpCode.ILSpy.Xaml.BamlResourceNodeFactory" ||
                    if(type.FullName == "ICSharpCode.ILSpy.TreeNodes.IResourceNodeFactory" ||
                        type.FullName.Contains("Menu") ||
                        type.FullName == "ICSharpCode.ILSpy.Language" ||
                        type.FullName.Contains("Metadata") ||
                        type.FullName.Contains("Attribute") ||
                        type.FullName.Contains("Xaml"))
                        skipList.Add(type);
                    //skipList.AddRange(Methods(type).Where(m => m.Name == "MethodA"));
                }
            }
            EntryPoints = skipList;
        }

        private void SetAccessedByName()
        {
            



        }

        private void SetInvokeByName()
        {
            InvokedByN = new Dictionary<Method, int>();

            // TODO: move to input configuration?


            //var typeType = Types(ResolveAssembly(typeof(System.Type).Module.FullyQualifiedName)).First(t => t.FullName == "System.Type");
            //var methods = Methods(typeType).Where(m => m.Name == "GetMethod" && m.IsPublic && m.Parameters.ElementAt(0).FullName == "System.String" && m.ParametersCount == 1);

            //foreach (var method in methods)
            //    InvokedByN.Add(method, 1);

            //var assemblyType = corlib.GetType("System.Reflection.Assembly");
            //var assemblyMethods = assemblyType.GetMethods().Where(m => m.HasParameters && m.Parameters[0].ParameterType.FullName == "System.String");


            ////foreach (var method in assemblyMethods)
            ////    InvokeByName.Add(new Method(method), 1);


            //var resourcesType = Types(ResolveAssembly("mscorlib")).First(t => t.FullName == "System.Resources.ResourceManager");
            //var resourceManagerMethods = Methods(resourcesType).Where(m => m.IsConstructor && m.ParametersCount > 0 && m.Parameters.ElementAt(0).FullName == "System.String");
            
            //foreach (var method in resourceManagerMethods)
            //    InvokedByName.Add(method, 1);                                    
        }

   
    }
}
