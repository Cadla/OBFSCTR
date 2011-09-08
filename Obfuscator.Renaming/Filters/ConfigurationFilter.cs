using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Renaming.Filters
{
    public class ConfigurationFilter : IFilter
    {
        public bool ShouldSkip(Mono.Cecil.IMemberDefinition member)
        {
            return false;
        }


    }
}
