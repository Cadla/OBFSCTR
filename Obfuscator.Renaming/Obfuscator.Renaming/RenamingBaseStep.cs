using Obfuscator.Common;
using Obfuscator.Common.Steps;

namespace Obfuscator.Renaming
{
    internal abstract class RenamingBaseStep : BaseStep
    {
        RenamingContext _context;

        public new RenamingContext Context
        {
            get
            {
                return _context;
            }
        }

        public override void Process(ObfuscationContext context)
        {
            _context = context as RenamingContext;

            base.Process(context);
        }
    }
}
