using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Common;
using Mono.Cecil;

namespace Obfuscator.Renaming
{
    internal class RenamingContext : ObfuscationContext
    {
        public Dictionary<AssemblyNameDefinition, Dictionary<IMemberDefinition, string>> DefinitionsRenameMap
        {
            get;
            private set;
        }

        public RenamingContext(ObfuscationContext obfuscationContext)
            : base(obfuscationContext)
        {
            DefinitionsRenameMap = new Dictionary<AssemblyNameDefinition, Dictionary<IMemberDefinition, string>>();
        }
    }
}
