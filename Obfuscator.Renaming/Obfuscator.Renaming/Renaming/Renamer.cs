using System.CodeDom.Compiler;
using System.Collections.Generic;
using Mono.Cecil;
using Obfuscator.Renaming.NameGenerators;

namespace Obfuscator.Renaming
{
    public class Renamer
    {
        INameGenerator _nameGenerator;
        Dictionary<IMemberDefinition, string> _definitionsMap;
        CodeDomProvider _codeProvider;

        public Dictionary<IMemberDefinition, string> DefinitionsMap
        {
            get { return _definitionsMap; }
        }

        public Renamer(INameGenerator nameGenerator)
        {
            _nameGenerator = nameGenerator;
            _definitionsMap = new Dictionary<IMemberDefinition, string>();
            _codeProvider = CodeDomProvider.CreateProvider("CSharp");
        }

        public string MapDefinition(IMemberDefinition member)
        {
            if (member.DeclaringType != null)
                return MapMemberDefinition(member);
            return MapTypeDefinition(member as TypeDefinition);
        }

        public bool TryGetMappedName(IMemberDefinition member, out string name)
        {
            return DefinitionsMap.TryGetValue(member, out name);
        }

        //TODO Same fully-qualified type names in two assemblies http://msdn.microsoft.com/en-us/library/ms173212.aspx
        private string MapMemberDefinition(IMemberDefinition member)
        {

            string newName = _codeProvider.CreateValidIdentifier(_nameGenerator.GetMemberName(member));
#if APPEND
            DefinitionsMap[member] = member.Name + newName;
            return member.Name + newName;
#else
            DefinitionsMap[member] = newName;
            return newName;
#endif
        }

        private string MapTypeDefinition(TypeDefinition type)
        {
            if (type.IsNested)
                return MapMemberDefinition(type);

            string newName = _codeProvider.CreateValidIdentifier(_nameGenerator.GetTypeName(type));
#if APPEND
            DefinitionsMap[type] = type.Name + newName;
            return type.Name + newName;
#else
            DefinitionsMap[type] = newName;
            return newName;
#endif
        }
    }
}
