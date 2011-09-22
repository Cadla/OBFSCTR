using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Renaming
{
    public interface INameGenerator
    {
        bool KeepNamespaces { get; }

        string GetMemberName(IMemberDefinition member);
                
        string GetTypeName(TypeDefinition type);
    }

}
