using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;

namespace Obfuscator.Renaming
{
    public class TypeMap //: NullAssemblyVisitor
    {
        IList<AssemblyDefinition> _assemblies;

        IDictionary<string, IList<MethodDefinition>> relatedMethods_;

        public TypeMap(AssemblyDefinition assembly) 
        {
            _assemblies.Add(assembly);
        }

        public TypeMap(IList<AssemblyDefinition> assemblies)
        {
            _assemblies = assemblies;
        }

        public IList<MethodDefinition> GetRelatedMethods(MethodDefinition method)
        {
            var result = new List<MethodDefinition>();



            return result;
        }

        
    }
}
