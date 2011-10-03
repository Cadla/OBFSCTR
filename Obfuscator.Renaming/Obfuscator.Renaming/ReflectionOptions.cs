using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Renaming
{    
    [Flags]
    public enum ReflectionOptions
    {
        None = 0x0,        
        Types = 0x1,
        Methods = 0x2,
        Fields = 0x4,
        NestedTypes = 0x8,
        Properties = 0x10,
        Events = 0x20,
    }
}
