using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Obfuscator.Steps
{
    public class SaveRenameMap : BaseStep
    {
        static StreamWriter writer = new StreamWriter("output.txt", false);
        
        protected override void ProcessAssembly(Mono.Cecil.AssemblyDefinition assembly)
        {
            
            var renameMap = Context.DefinitionsRenameMap[assembly.Name];

            writer.WriteLine("Assembly : {0}", assembly.Name);
            foreach (var member in renameMap)
            {
                writer.WriteLine("\tMember {0} {1} : {2}", member.Key.MetadataToken.TokenType, member.Key.FullName, member.Value);
            }
        }
    }
}
