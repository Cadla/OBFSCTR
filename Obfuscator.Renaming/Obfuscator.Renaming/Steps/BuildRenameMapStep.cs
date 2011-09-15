using System.Collections.Generic;
using Mono.Cecil;
using Obfuscator.Utils;
using System.IO;
using System.Linq;
using Obfuscator.Renaming;

namespace Obfuscator.Steps.Renaming
{
    public class BuildRenameMapStep : BaseStep
    {        
        // TODO read: http://www.unicode.org/unicode/reports/tr15/tr15-18.htm to get allowed characters    
        private const string ALPHABET = "ABCDEFGHIJKLMNOPRSTUWXYZ";
                
        AssemblyVisitor _visitor;

        public BuildRenameMapStep()
        {
            _visitor = new AssemblyVisitor();                       
        }

        protected override void  ProcessAssembly(AssemblyDefinition assembly)
        { 	      
            Renamer renamer = new Renamer(new NameGenerator(ALPHABET), Context.Options);
            RenameMapVisitor renameMapVisitor = new RenameMapVisitor(renamer, Context.InputConfiguration);
            _visitor.ConductVisit(assembly, renameMapVisitor);

            Context.DefinitionsRenameMap[assembly.Name] = renamer.DefinitionsMap;
            Context.ResourcesRenameMap[assembly.Name] = renamer.ResourcesNames;
            Context.ReferencesRenameMap[assembly.Name] = renamer.References;
        }
    }
}
