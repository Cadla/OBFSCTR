using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Renaming.Filters
{
    public interface IFilter
    {
        bool ShouldSkip(IMemberDefinition member);
    }
}
