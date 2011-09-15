using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscator.Configuration.COM
{
    // TODO
    public class CustomAttribute
    {
        Mono.Cecil.CustomAttribute _customAttribute;

        internal CustomAttribute(Mono.Cecil.CustomAttribute customAttribute)
        {
            _customAttribute = customAttribute;                        
        }

        public COM.Type AttributeType
        {
            get
            {
                return new COM.Type(_customAttribute.AttributeType);
            }
        }       

    }
}
