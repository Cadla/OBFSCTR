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
        bool InvokesByName(MethodReference method, out int paramIndex);        
    }
}

