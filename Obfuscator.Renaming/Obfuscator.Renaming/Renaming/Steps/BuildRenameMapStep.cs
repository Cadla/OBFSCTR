using System.Collections.Generic;
using Mono.Cecil;
using Obfuscator.Utils;
using System.IO;
using System.Linq;
using Obfuscator.Renaming;
using System.Text;
using Obfuscator.Common.Steps;
using Obfuscator.Renaming.Visitors;
using Obfuscator.Renaming.NameGenerators;
using Obfuscator.Renaming.Steps;

namespace Obfuscator.Renaming.Steps
{
    internal class BuildRenameMapStep : RenamingBaseStep
    {                
        AssemblyVisitor _visitor;
        RenamingOptions _options;
        INameGenerator _nameGenerator;

        public BuildRenameMapStep(INameGenerator nameGenerator)
        {
            _visitor = new AssemblyVisitor();            
            _nameGenerator = nameGenerator;
        }

        protected override void  ProcessAssembly(AssemblyDefinition assembly)
        {                    
            Renamer renamer = new Renamer(_nameGenerator);
            DefinitionsMapVisitor renameMapVisitor = new DefinitionsMapVisitor(renamer, Context.Filter);
            _visitor.ConductVisit(assembly, renameMapVisitor);
            Context.DefinitionsRenameMap[assembly.Name] = renamer.DefinitionsMap;
        }
    }
}
