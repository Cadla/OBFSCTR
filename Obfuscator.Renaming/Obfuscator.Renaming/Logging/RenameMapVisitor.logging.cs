using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Renaming
{
    public partial class RenameMapVisitor
    {
        partial void logVisitingMember(IMemberDefinition member)
        {
            //Console.WriteLine("Visiting {0} {1}", member.MetadataToken.TokenType, member.FullName);
        }

        partial void logSkipingMember(IMemberDefinition member, string message)
        {
            Console.WriteLine("Skipping {0} {1}, justification: {2}", member.MetadataToken.TokenType, member.FullName, message);
        }

        partial void logRenamingDefinition(IMemberDefinition member, string newName)
        {
            Console.WriteLine("Renaming {0} {1} to : {2}", member.MetadataToken.TokenType, member.FullName, newName);
        }

        partial void logRenamingResource(Resource resource, string newName)
        {
            Console.WriteLine("Renaming {0} {1} to : {2}", "resource", resource.Name, newName);
        }
    }
}
