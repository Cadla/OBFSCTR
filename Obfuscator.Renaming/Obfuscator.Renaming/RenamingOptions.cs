using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator
{
    [Flags]
    public enum RenamingOptions
    {
        Default = 0x0,
        KeepNamespaces = 0x1,
        CTSCompliance = 0x2,
        Reflection = 0x4,
        SaveRenameMap = 0x8
    }
}
