using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Common
{
    public interface IMemberFilter
    {
        bool ShouldSkip(IMemberDefinition token);        
    }

    public class DefaultFilter : IMemberFilter
    {
        public bool ShouldSkip(IMemberDefinition token)
        {
            return false;
        }
    }
}

