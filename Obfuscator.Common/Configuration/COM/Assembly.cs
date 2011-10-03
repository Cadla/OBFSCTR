using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Configuration.COM
{
    public class Assembly
    {
        private AssemblyDefinition _assemblyDefinition;

        internal AssemblyDefinition AssemblyDefinition
        {
            get
            {
                return _assemblyDefinition;
            }
        }

        internal Assembly(AssemblyDefinition assembly)
        {
            _assemblyDefinition = assembly;
        }

        public string Name
        {
            get
            {
                return _assemblyDefinition.Name.Name;
            }
        }

        public override string ToString()
        {
            return _assemblyDefinition.ToString();
        }
    }
}
