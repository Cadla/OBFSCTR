using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;

namespace Obfuscator.Renaming
{
    public class TypeTree //: NullAssemblyVisitor
    {
        IList<AssemblyDefinition> _assemblies;

        public TypeTree(AssemblyDefinition assembly) 
        {
            _assemblies.Add(assembly);
        }

        public TypeTree(IList<AssemblyDefinition> assemblies)
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
