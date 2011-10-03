using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Obfuscator.MetadataBuilder
{
    public static class MetadataBuilderHelper
    {
        public static TypeDefinition CreateNewType(TypeDefinition sourceType, ReferenceResolver resolver)
        {
            TypeDefinition newType = new TypeDefinition(sourceType.Namespace, sourceType.Name, sourceType.Attributes)
            {
                PackingSize = sourceType.PackingSize,
                ClassSize = sourceType.ClassSize,
            };

            CopyGenericParameters(sourceType, newType, resolver);

            if (sourceType.BaseType != null)
            {
                newType.BaseType = resolver.ReferenceType(sourceType.BaseType, newType);
            }

            CopyCustomAttributes(sourceType, newType, resolver);
            return newType;
        }

        public static void CopyGenericParameters(IGenericParameterProvider source, IGenericParameterProvider target, ReferenceResolver resolver)
        {
            if (!source.HasGenericParameters)
                return;

            foreach (var parameter in source.GenericParameters)
                target.InjectGenericParameter(parameter, resolver);
        }

        public static void CopyCustomAttributes(ICustomAttributeProvider source, ICustomAttributeProvider target, ReferenceResolver resolver)
        {
            if (!source.HasCustomAttributes)
                return;

            foreach (var attribute in source.CustomAttributes)
                target.InjectCustomAttribute(attribute, resolver);
        }

        public static void CopyCustomAttributeArguments(Collection<Mono.Cecil.CustomAttributeArgument> source, Collection<Mono.Cecil.CustomAttributeArgument> target, ReferenceResolver resolver)
        {
            foreach (var argument in source)
            {
                var argumentType = resolver.ReferenceType(argument.Type);
                target.Add(new CustomAttributeArgument(argumentType, argument.Value));
            }
        }

        public static void CopyCustomAttributeNamedArguments(Collection<Mono.Cecil.CustomAttributeNamedArgument> source,
            Collection<Mono.Cecil.CustomAttributeNamedArgument> target, ReferenceResolver resolver)
        {
            foreach (var namedArgument in source)
            {
                var argumentType = resolver.ReferenceType(namedArgument.Argument.Type);
                CustomAttributeArgument argument = new CustomAttributeArgument(argumentType, namedArgument.Argument.Value);

                target.Add(new Mono.Cecil.CustomAttributeNamedArgument(namedArgument.Name, argument));
            }
        }

        public static void CopySecurityAttributes(SecurityDeclaration source, SecurityDeclaration target, ReferenceResolver resolver)
        {
            if (!source.HasSecurityAttributes)
                return;

            foreach (var attribute in source.SecurityAttributes)
                target.InjectSecurityAttribute(attribute, resolver);
        }

        public static void CopySecurityDeclarations(ISecurityDeclarationProvider source, ISecurityDeclarationProvider target, ReferenceResolver resolver)
        {
            if (!source.HasSecurityDeclarations)
                return;

            foreach (var declaration in source.SecurityDeclarations)
                target.InjectSecurityDeclaration(declaration, resolver);
        }
     
    }
}
