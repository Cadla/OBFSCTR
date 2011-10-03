using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;
using Mono.Collections.Generic;

namespace Obfuscator.MetadataBuilder
{
    public static class ICustomAttributeProviderExtensions
    {
        public static CustomAttribute InjectCustomAttribute(this ICustomAttributeProvider provider, CustomAttribute attribute, ReferenceResolver resolver)
        {            
            if (attribute == null)
                throw new ArgumentNullException("attribute");
            if (resolver == null)
                throw new ArgumentNullException("resolver");     

            TypeReference attributeType = resolver.ReferenceType(attribute.AttributeType);

            // no context required as attributes cannot be generic
            MethodReference constructor = resolver.ReferenceMethod(attribute.Constructor);

            CustomAttribute newAttribute;
            if ((newAttribute = Helper.GetCustomAttribute(provider.CustomAttributes, attribute)) != null)
                return newAttribute;

            newAttribute = new CustomAttribute(constructor);//, attr.GetBlob());
            provider.CustomAttributes.Add(newAttribute);

            MetadataBuilderHelper.CopyCustomAttributeArguments(attribute.ConstructorArguments, newAttribute.ConstructorArguments, resolver);

            MetadataBuilderHelper.CopyCustomAttributeNamedArguments(attribute.Fields, newAttribute.Fields, resolver);

            MetadataBuilderHelper.CopyCustomAttributeNamedArguments(attribute.Properties, newAttribute.Properties, resolver);
     
            return newAttribute;
        }
    }
}
