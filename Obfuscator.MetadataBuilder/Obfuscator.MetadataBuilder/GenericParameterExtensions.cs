using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.MetadataBuilder
{
    public static class GenericParameterExtensions
    {
        public static void InjectGenericParameter(this IGenericParameterProvider owner, GenericParameter parameter, ReferenceResolver resolver)
        {
            GenericParameter newParameter = new GenericParameter(parameter.Name, owner)
            {
                Attributes = parameter.Attributes
            };

            owner.GenericParameters.Add(newParameter);

            MetadataBuilderHelper.CopyCustomAttributes(newParameter, parameter, resolver);

            foreach (var constraint in parameter.Constraints)
            {
                if (owner is MethodReference)
                    newParameter.Constraints.Add(resolver.ReferenceType(constraint, owner, ((MethodReference)owner).DeclaringType));
                else
                    newParameter.Constraints.Add(resolver.ReferenceType(constraint, owner));
            }
        }
    }
}
