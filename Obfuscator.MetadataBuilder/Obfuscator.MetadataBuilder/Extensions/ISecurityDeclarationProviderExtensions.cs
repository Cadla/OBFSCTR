using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.MetadataBuilder.Extensions
{
    public static class ISecurityDeclarationProviderExtensions
    {
        public static void InjectSecurityDeclaration(this ISecurityDeclarationProvider target, SecurityDeclaration declaration, ReferenceResolver resolver)
        {
            var newDeclaration = new SecurityDeclaration(declaration.Action);
            MetadataBuilderHelper.CopySecurityAttributes(newDeclaration, declaration, resolver);                
            target.SecurityDeclarations.Add(newDeclaration);
        }
    }
}
