using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator
{
    [Flags]
    public enum ObfuscationOptions
    {
        Default,
        KeepNamespaces,
        CTSCompliance
    }
}
