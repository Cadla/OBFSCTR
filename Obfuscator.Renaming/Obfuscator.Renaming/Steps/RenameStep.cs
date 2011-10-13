using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Common.Steps;
using Obfuscator.Common;
using Obfuscator.Renaming.Steps;
using Obfuscator.Renaming.Reflection.Steps;
using Obfuscator.Renaming.NameGenerators;

namespace Obfuscator.Renaming.Steps
{
    public class RenameStep : BaseStep
    {
        private const string ALPHABET = "ABCDEFGHIJKLMNOPRSTUWXYZ";

        RenamingOptions _renamingOptions;
        ReflectionOptions _reflectionOptions;
        Pipeline _pipeline;

        public RenameStep(RenamingOptions renamingOptions, ReflectionOptions reflectionOptions)
        {
            _renamingOptions = renamingOptions;
            _reflectionOptions = reflectionOptions;

            bool keepNamespaces = _renamingOptions.HasFlag(RenamingOptions.KeepNamespaces);
            INameGenerator nameGenerator = GetNameGenerator(new StringGenerator(ALPHABET), keepNamespaces);

            _pipeline = new Pipeline();
            _pipeline.AppendStep(new FillMethodImplTablesStep());
            _pipeline.AppendStep(new BuildRenameMapStep(nameGenerator));

            if (_renamingOptions.HasFlag(RenamingOptions.Reflection))
                _pipeline.AppendStep(new InjectReflectionMethodProxies(_reflectionOptions, keepNamespaces));

            if (_renamingOptions.HasFlag(RenamingOptions.SaveRenameMap))
                _pipeline.AppendStep(new SaveRenameMap());

            _pipeline.AppendStep(new RenameReferencesStep(_renamingOptions));
            _pipeline.AppendStep(new RenameDefinitionsStep(_renamingOptions));
        }

        protected override void Process()
        {
            _pipeline.Process(new RenamingContext(Context));
        }

        private INameGenerator GetNameGenerator(StringGenerator stringGenerator, bool keepNamespaces)
        {
            if (_renamingOptions.HasFlag(RenamingOptions.CTSCompliance))
                return new CTSNameGenerator(stringGenerator, keepNamespaces);
            else
                return new CLSNameGenerator(stringGenerator, keepNamespaces);
        }
    }
}
