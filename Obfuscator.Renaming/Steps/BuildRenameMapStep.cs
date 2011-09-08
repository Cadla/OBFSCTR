using System.Collections.Generic;
using Mono.Cecil;
using Obfuscator.Renaming.Filters;
using Obfuscator.Renaming.Visitors;
using Obfuscator.Utils;
using System.IO;

namespace Obfuscator.Renaming.Steps
{
    public class BuildRenameMapStep : BaseStep
    {        
        // TODO read: http://www.unicode.org/unicode/reports/tr15/tr15-18.htm to get allowed characters    
        private const string ALPHABET = "ABCDEFGHIJKLMNOPRSTUWXYZ";
        
        Dictionary<AssemblyNameDefinition, Renamer> _renamers;
        AssemblyVisitor _visitor;

        public BuildRenameMapStep()
        {
            _visitor = new AssemblyVisitor();           
            _renamers = new Dictionary<AssemblyNameDefinition, Renamer>();
        }

        protected override void  ProcessAssembly(AssemblyDefinition assembly)
        { 	      
            Renamer renamer = new Renamer(new NameGenerator(ALPHABET));
            RenameMapVisitor renameMapVisitor = new RenameMapVisitor(renamer, new ConfigurationFilter());
            _visitor.ConductVisit(assembly, renameMapVisitor);
            _renamers[assembly.Name] = renamer;          
        }

        protected override void EndProcess()
        {
            Context.Renamers = _renamers;
        }

  
    }
}
