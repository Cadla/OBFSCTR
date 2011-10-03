using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;

namespace Obfuscator.MetadataBuilder
{
    public static class ModuleDefinitionExtensions
    {
        public static TypeDefinition InjectType(this ModuleDefinition module, TypeDefinition sourceType, ReferenceResolver resolver)
        {
            TypeDefinition newType = null;
            if (Helper.TryGetType(module.Types, sourceType, ref newType))
                return newType;

            newType = MetadataBuilderHelper.CreateNewType(sourceType, resolver);            
            module.Types.Add(newType);

            newType.InjectTypeMembers(sourceType, resolver);
            return newType;                        
        }

        public static Resource InjectResource(this ModuleDefinition module, Resource resource)
        {
            if (!module.Resources.Contains(resource))
                module.Resources.Add(resource);
            return resource;
        }
    }
}
