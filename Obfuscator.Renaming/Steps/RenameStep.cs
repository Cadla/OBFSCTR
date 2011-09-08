using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Renaming.Steps
{
    public class RenameStep : BaseStep
    {
        protected override void Process()
        {
            foreach (var assembly in Context.Renamers)
            {
                assembly.Value.RenameAll();
            }
        }
    }
}
