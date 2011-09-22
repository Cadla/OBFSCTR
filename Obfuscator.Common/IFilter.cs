using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator
{
    public interface IFilter
    {
        bool ShouldSkip(IMemberDefinition token);
        bool ShouldKeepNamespaces(AssemblyDefinition assembly);
        bool AccessedByName(TypeReference type, out int nameIndex);        
        bool InvokedByName(MethodReference method, out int nameIndex, out int typeInstanceIndex);        
    }
}

