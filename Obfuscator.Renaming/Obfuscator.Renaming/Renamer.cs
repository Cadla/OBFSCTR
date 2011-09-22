using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.IO;
using Mono.Cecil.Cil;
using Obfuscator.Utils;

namespace Obfuscator.Renaming
{
    // Make two renamers out of this
    public class Renamer
    {
        INameGenerator _scopeNameGenerator;
        ObfuscationOptions _options;

        public Dictionary<IMemberDefinition, string> DefinitionsMap
        {
            get;
            private set;
        }
        
        public Dictionary<Resource, string> ResourcesNames
        {
            get;
            private set;
        }

        //TODO: Should save references with respect to the referenced member definition assembly
        public Dictionary<MemberReference, string> References
        {
            get;
            private set;
        }

      
        public Renamer(IStringGenerator nameGenerator, ObfuscationOptions options)
        {
            _scopeNameGenerator = new CTSNameGenerator(nameGenerator, options.HasFlag(ObfuscationOptions.KeepNamespaces));

            _options = options;

            DefinitionsMap = new Dictionary<IMemberDefinition, string>();
            ResourcesNames = new Dictionary<Resource, string>();

            References = new Dictionary<MemberReference, string>();
        }

        public string MapDefinition(IMemberDefinition member)
        {
            if (member.DeclaringType != null)
                return MapMemberDefinition(member);
            return MapTypeDefinition(member as TypeDefinition);
        }

        public void MapReference(MemberReference reference)
        {
            if (References.ContainsKey(reference))
                return;

            IMetadataScope scope;
            scope = GetScope(reference);

            if (Helper.IsCoreAssemblyName(scope.Name))
                return;

            References.Add(reference, String.Empty);
        }

        private static IMetadataScope GetScope(MemberReference reference)
        {
            IMetadataScope scope;
            if (reference.DeclaringType == null)
                scope = ((TypeReference)reference).Scope;
            else
                scope = reference.DeclaringType.Scope;
            return scope;
        }

        public string MapResource(Resource resource)
        {
            string name = Path.GetFileNameWithoutExtension(resource.Name);
            IMemberDefinition member;
            if ((member = GetMappedMember(name)) != null)
            {
                string suffix = resource.Name.Substring(name.Length);
                var newName = DefinitionsMap[member];
                newName = newName + suffix;
                if (_options.HasFlag(ObfuscationOptions.KeepNamespaces) && member.DeclaringType == null)
                    newName = ((TypeDefinition)member).Namespace + '.' + newName;

                ResourcesNames[resource] = newName ;
                return newName;
            }
            return resource.Name;
        }

        //TODO Same fully-qualified type names in two assemblies http://msdn.microsoft.com/en-us/library/ms173212.aspx
        private string MapMemberDefinition(IMemberDefinition member)
        {
            string newName = _scopeNameGenerator.GetMemberName(member);
            DefinitionsMap[member] = member.Name + newName;
            return member.Name + newName;
        }

        private string MapTypeDefinition(TypeDefinition type)
        {
            if (type.IsNested)
                return MapMemberDefinition(type);

            string newName = _scopeNameGenerator.GetTypeName(type);
            DefinitionsMap[type] = type.Name + newName;
            return type.Name + newName;
        }

        public bool TryGetMappedName(IMemberDefinition member, out string name)
        {
            return DefinitionsMap.TryGetValue(member, out name);
        }

        public bool TryGetMappedName(Resource resource, out string name)
        {
            return ResourcesNames.TryGetValue(resource, out name);
        }
   
        private IMemberDefinition GetMappedMember(string memberName)
        {
            IMemberDefinition member;
            if ((member = DefinitionsMap.Keys.SingleOrDefault(m => m.FullName == memberName)) != null)
            {
                return member;
            }
            return null;
        }
    }
}
