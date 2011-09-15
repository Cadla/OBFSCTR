using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Configuration.COM;

namespace Obfuscator.Configuration
{

    public class DefaultConfiguration : InputConfiguration
    {
        public override ObfuscationOptions GetOptions()
        {
            return ObfuscationOptions.Default;
        }

        public override IList<Member> Skip()
        {
            var skipList = new List<Member>();
            foreach (var assembly in Assemblies)
            {
                var entryTypes =
                    from t in Types(assembly)
                    where Methods(t).Any(m => m.Name == "Main")
                    select t;
                skipList.AddRange(entryTypes);
            }
            return skipList;
        }

        public override IList<Method> KeepStrings()
        {
            throw new NotImplementedException();
        }

        public override IList<Assembly> KeepNamespaces()
        {
            throw new NotImplementedException();
        }
    }
}
