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

        public DOM.Type AttributeType
        {
            get
            {
                return new DOM.Type(_customAttribute.AttributeType);
            }
        }       

    }
}
