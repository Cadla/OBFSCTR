using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Configuration.COM
{
    public class Property : Member
    {
        PropertyDefinition _propertyDefinition;

       
        internal PropertyDefinition PropertyDefinition
        {
            get
            {
                return _propertyDefinition;
            }
        }

        public Property(PropertyDefinition propertyDefinition)
            : base(propertyDefinition)
        {
            _propertyDefinition = propertyDefinition;
        }

        public Type PropertyType
        {
            get
            {
                return new COM.Type(_propertyDefinition.DeclaringType);
            }
        }

        public bool HasParameters
        {
            get
            {
                return _propertyDefinition.HasParameters;
            }
        }

        //public Method SetMethod { get; internal set; }
        //public Method GetMethod { get; internal set; }
    }
}
