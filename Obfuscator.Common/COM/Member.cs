using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Configuration.COM
{
    public class Member
    {
        private IMemberDefinition _memberDefinition;

        internal IMemberDefinition MemberDefinition
        {
            get
            {
                return _memberDefinition;
            }
        }

        internal Member(IMemberDefinition memberDefinition)
        {
            _memberDefinition = memberDefinition;            
        }
        
        public IEnumerable<CustomAttribute> CustomAttributes
        {
            get
            {
                return _memberDefinition.CustomAttributes.Select(m => new CustomAttribute(m));
            }

        }

        public string Name
        {
            get
            {
                return _memberDefinition.Name;
            }
        }
        public string FullName
        {
            get
            {
                return _memberDefinition.FullName;
            }
        }

        public Type DeclaringType
        {
            get
            {
                return new COM.Type(_memberDefinition.DeclaringType);
            }
        }

        public override bool Equals(object obj)
        {
            return ((Member)obj)._memberDefinition.Equals(this._memberDefinition);
        }

    }
}
