using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.MetadataBuilder
{
    public static class SecurityDeclarationExtensions
    {
        public static void InjectSecurityAttribute(this SecurityDeclaration target, SecurityAttribute attribute, ReferenceResolver resolver)
        {
            var attributeType = resolver.ReferenceType(attribute.AttributeType);
            var newAttribute = new SecurityAttribute(attributeType);

            MetadataBuilderHelper.CopyCustomAttributeNamedArguments(attribute.Fields, newAttribute.Fields, resolver);
            MetadataBuilderHelper.CopyCustomAttributeNamedArguments(attribute.Properties, newAttribute.Properties, resolver);

            target.SecurityAttributes.Add(newAttribute);
        }
    }
}
