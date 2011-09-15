using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Configuration.COM
{
    public class Type : Member
    {
        TypeDefinition _typeDefinition;

        internal TypeDefinition TypeDefinition
        {
            get
            {
                return _typeDefinition;
            }
        }

        internal Type(TypeDefinition typeDefinition)
            : base(typeDefinition)
        {
            _typeDefinition = typeDefinition;
        }

        internal Type(TypeReference typeReference)
            : base(typeReference.Resolve())
        {
            _typeDefinition = typeReference.Resolve();
        }

        public Assembly Assembly
        {
            get
            {
                return new Assembly(_typeDefinition.Module.Assembly);
            }
        }

        public int GenericParametersCount
        {
            get
            {
                return _typeDefinition.GenericParameters.Count;
            }
        }

        public bool IsNested
        {
            get
            {
                return _typeDefinition.IsNested;
            }

        }
        public bool IsInterface
        {
            get
            {
                return _typeDefinition.IsInterface;
            }
        }

        public bool IsSealed
        {
            get
            {
                return _typeDefinition.IsSealed;
            }
        }

        public string Namespace
        {
            get
            {
                return _typeDefinition.Namespace;
            }
        }

        public bool IsPublic
        {
            get
            {
                return _typeDefinition.IsPublic;
            }
        }

        public bool IsNotPublic
        {
            get
            {
                return _typeDefinition.IsNotPublic;
            }
        }

        public bool IsNestedInternal
        {
            get
            {
                return _typeDefinition.IsNestedAssembly;
            }
        }

        public bool IsNestedPublic
        {
            get
            {
                return _typeDefinition.IsNestedPublic;
            }
        }

        public bool IsNestedPrivate
        {
            get
            {
                return _typeDefinition.IsNestedPrivate;
            }
        }

        public bool IsNestedProtected
        {
            get
            {
                return _typeDefinition.IsNestedFamily;
            }
        }

        public bool IsNestedProtectedInternal
        {
            get
            {
                return _typeDefinition.IsNestedFamilyOrAssembly;
            }
        }
    }
}
