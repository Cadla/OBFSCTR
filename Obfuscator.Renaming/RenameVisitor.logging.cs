using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Renaming
{
    public partial class RenameVisitor
    {
        partial void logVisitingMember(IMemberDefinition member)
        {
     //       Console.WriteLine("Visiting {0} {1}", member.MetadataToken.TokenType, member.FullName);
        }

        partial void logSkipingMember(IMemberDefinition member, string message)
        {
            Console.WriteLine("Skipping {0} {1}, justification: {2}", member.MetadataToken.TokenType, member.FullName,message);
        }
    }
}
