using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Configuration.COM
{
    public class Field : Member
    {
        FieldDefinition _fieldDefinition;

        internal FieldDefinition FieldDefinition
        {
            get
            {
                return _fieldDefinition;
            }
        }

        internal Field(FieldDefinition fieldDefinition)
            : base(fieldDefinition)
        {
            _fieldDefinition = fieldDefinition;
        }

        public Type FieldType
        {
            get
            {
                return new DOM.Type(_fieldDefinition.FieldType);
            }
        }

        public bool IsStatic
        {
            get
            {
                return _fieldDefinition.IsStatic;
            }
        }

        public bool IsInternal
        {
            get
            {
                return _fieldDefinition.IsAssembly;
            }
        }

        public bool IsPublic
        {
            get
            {
                return _fieldDefinition.IsPublic;
            }
        }

        public bool IsPrivate
        {
            get
            {
                return _fieldDefinition.IsPublic;
            }
        }

        public bool IsProtected
        {
            get
            {
                return _fieldDefinition.IsFamily;
            }
        }

        public bool IsProtectedInternal
        {
            get
            {
                return _fieldDefinition.IsFamilyOrAssembly;
            }
        }

        // public IList<CustomAttribute> CustomAttributes;
    }
}
