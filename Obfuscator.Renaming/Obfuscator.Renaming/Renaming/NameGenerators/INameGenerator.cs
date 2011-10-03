using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Renaming.NameGenerators
{
    public interface INameGenerator
    {
        string GetMemberName(IMemberDefinition member);
                
        string GetTypeName(TypeDefinition type);
    }
}
