using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;

namespace Obfuscator.MetadataBuilder
{
    public static class BuildersHelper
    {
        public static bool NeedsImport(ModuleDefinition module, MemberReference member)
        {
            if (Helper.AreSame(module, member.Module))
                return false;
            return true;
        }    
    }
}
