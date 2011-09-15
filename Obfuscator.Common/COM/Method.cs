using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Configuration.COM
{
    public class Method : Member
    {
        MethodDefinition _methodDefinition;

        internal MethodDefinition MethodDefinition
        {
            get
            {
                return _methodDefinition;
            }
        }

        internal Method(MethodDefinition methodDefinition) :
            base(methodDefinition)
        {
            _methodDefinition = methodDefinition;
        }

        internal Method(MethodReference methodReference) :
            base(methodReference.Resolve())
        {
            _methodDefinition = methodReference.Resolve();
        }

        public int GenericParametersCount
        {
            get
            {
                return _methodDefinition.GenericParameters.Count;
            }
        }

        public int ParametersCount
        {
            get
            {
                return _methodDefinition.Parameters.Count;
            }
        }

        public IEnumerable<Type> Parameters
        {
            get
            {
                // TODO what if cannot resolve?
                return _methodDefinition.Parameters.Select(p => new COM.Type(p.ParameterType));
            }
        }

        public Type ReturnType
        {
            get
            {
                return new COM.Type(_methodDefinition.ReturnType);
            }
        }

        public bool IsAbstract
        {
            get
            {
                return _methodDefinition.IsAbstract;
            }
        }

        public bool IsConstructor
        {
            get
            {
                return _methodDefinition.IsConstructor;
            }
        }

        public bool IsFinal
        {
            get
            {
                return _methodDefinition.IsFinal;
            }
        }

        public bool IsVirtual
        {
            get
            {
                return _methodDefinition.IsVirtual;
            }
        }

        public bool IsStatic
        {
            get
            {
                return _methodDefinition.IsStatic;
            }
        }

        public bool IsInternal
        {
            get
            {
                return _methodDefinition.IsAssembly;
            }
        }

        public bool IsPublic
        {
            get
            {
                return _methodDefinition.IsPublic;
            }
        }

        public bool IsPrivate
        {
            get
            {
                return _methodDefinition.IsPublic;
            }
        }

        public bool IsProtected
        {
            get
            {
                return _methodDefinition.IsFamily;
            }
        }

        public bool IsProtectedInternal
        {
            get
            {
                return _methodDefinition.IsFamilyOrAssembly;
            }
        }

        public bool IsGetter
        {
            get
            {
                return _methodDefinition.IsGetter;
            }
        }

        public bool IsSetter
        {
            get
            {
                return _methodDefinition.IsSetter;
            }
        }

        public bool IsAddOn
        {
            get
            {
                return _methodDefinition.IsAddOn;
            }
        }

        public bool IsRemoveOn
        {
            get
            {
                return _methodDefinition.IsRemoveOn;
            }
        }
    }
}
