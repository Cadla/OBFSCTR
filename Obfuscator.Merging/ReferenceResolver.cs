﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace Merger
{
    public class ReferenceResolver
    {
        private static IDictionary<TypeReference, TypeReference> cache = new Dictionary<TypeReference, TypeReference>();

        ModuleDefinition module;
        Func<TypeReference, bool> isInternal;

        public ReferenceResolver()
        {

        }

        public ReferenceResolver(ModuleDefinition module, Func<TypeReference, bool> toImport)
        {
            Module = module;
            isInternal = toImport;
        }

        public Func<TypeDefinition, bool> Action
        {
            get;
            set;
        }

        public ModuleDefinition Module
        {
            get
            {
                return module;
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");

                cache.Clear();
                this.module = value;
            }
        }

        public TypeReference ReferenceType(TypeReference type, params IGenericParameterProvider[] paramProviders)
        {
            if(type == null)
                throw new ArgumentNullException("type");
            
            CheckState();

            TypeReference reference;

            if(TryToGetReferenceFromCache(type, out reference))
                return reference;

            if(type is TypeSpecification) //TODO || is open generic type ?
                reference = ReferenceTypeSpecification(type, paramProviders);

            else if(type.IsGenericParameter)
                reference = GetGenericParameter(type, paramProviders);

            else if(!isInternal(type))
            {
                reference = ImportType(type);
            }
            else
            {
                if((reference = module.GetType(type.Resolve().FullName)) == null)
                {
                    reference = Action(type.Resolve());
                    //throw new InvalidOperationException(String.Format("Type {0} doesn't exist", type.FullName));                   
                }
                reference = CreateTypeReference(reference, paramProviders);
            }

            cache[type] = reference;
            return reference;
        }

        public FieldReference ReferenceField(FieldReference field, params IGenericParameterProvider[] paramProviders)
        {
            if(field == null)
                throw new ArgumentNullException("field");

            CheckState();

            var declaringType = ReferenceType(field.DeclaringType, paramProviders);

            var providers = paramProviders.ToList();
            providers.Add(declaringType);

            var fieldType = ReferenceType(field.FieldType, providers.ToArray());

            return new FieldReference(field.Name, fieldType, declaringType);
        }

        public MethodReference ReferenceMethod(MethodReference method, params IGenericParameterProvider[] paramProviders)
        {
            if(method == null)
                throw new ArgumentNullException("method");

            // Console.WriteLine("\t\tRefernce method {0}", method.FullName);

            TypeReference declaringType = ReferenceType(method.DeclaringType, paramProviders);

            if(method.IsGenericInstance)
                return ReferenceGenericInstanceMethod((GenericInstanceMethod)method, paramProviders);

            var reference = new MethodReference(method.Name, method.ReturnType, declaringType)
            {
                CallingConvention = method.CallingConvention,
                HasThis = method.HasThis,
                ExplicitThis = method.ExplicitThis
            };

            CopyGenericParameters(method, reference);

            var providers = paramProviders.ToList();
            providers.Add(declaringType);
            providers.Add(reference);

            CopyParameters(method, reference, providers.ToArray());

            reference.ReturnType = ReferenceType(method.ReturnType, providers.ToArray());

            return reference;
        }

        private void CheckState()
        {
            if(module == null)
                throw new MemberAccessException("Module property was not initialized");
            if(Action == null)
                throw new MemberAccessException("Action property was not initialized");
        }

        private bool TryToGetReferenceFromCache(TypeReference type, out TypeReference typeReference)
        {
            typeReference = null;
            if(cache.ContainsKey(type))
            {
                typeReference = cache[type];

                //TODO Write custom GetHashCode method for TypeReference?
                if(type.IsValueType && !typeReference.IsValueType)
                    typeReference.IsValueType = true;
                return true;
            }
            return false;
        }

        private TypeReference ImportType(TypeReference type)
        {
            TypeReference imported;

            if((imported = module.Import(type)) != null)
            {
                return imported;
            }
            return null;
        }

        private TypeReference ReferenceTypeSpecification(TypeReference type, params IGenericParameterProvider[] paramProviders)
        {
            if(type.IsRequiredModifier)
            {
                var requiredModifier = (RequiredModifierType)type;
                var modifierType = ReferenceType(requiredModifier.ModifierType, paramProviders);
                return ReferenceType(requiredModifier.ElementType, paramProviders).MakeRequiredModifierType(modifierType);
            }
            else if(type.IsByReference)
            {
                return ReferenceType(((ByReferenceType)type).ElementType, paramProviders).MakeByReferenceType();
            }
            else if(type.IsArray)
            {
                return ReferenceType(((ArrayType)type).ElementType, paramProviders).MakeArrayType();
            }
            else if(type.IsGenericInstance)
            {
                var genericInstance = (GenericInstanceType)type;
                var elementType = ReferenceType(genericInstance.ElementType, paramProviders);
                return ReferenceGenericInstanceType(genericInstance, elementType, paramProviders);
            }
            //TODO PointerType, PinnedType, SentinelType, OptionalModifierType, SzArray ?
            else
            {
                throw new NotSupportedException(type.ToString());
            }
        }

        private TypeReference GetGenericParameter(TypeReference type, IGenericParameterProvider[] paramProviders)
        {
            var genericParameter = (GenericParameter)type;
            IGenericParameterProvider context = GetContext(genericParameter, paramProviders);
            return context.GenericParameters[genericParameter.Position];
        }

        private IGenericParameterProvider GetContext(GenericParameter parameter, IGenericParameterProvider[] paramProviders)
        {
            //GetElementProvider(ref paramProviders);
            paramProviders = paramProviders.Select(x => GetElementProvider(x)).ToArray();

            if(parameter.MetadataType == MetadataType.MVar)
                return paramProviders.
                    Where(x => x.GenericParameterType == GenericParameterType.Method).
                    First(x => Helper.AreSame((MethodReference)x, (MethodReference)parameter.Owner));

            else
                return paramProviders.
                    Where(x => x.GenericParameterType == GenericParameterType.Type).
                    First(x => Helper.AreSame((TypeReference)x, (TypeReference)parameter.Owner));
        }

        //private void GetElementProvider(ref IGenericParameterProvider[] paramProviders)
        //{
        //    HashSet<IGenericParameterProvider> providers = new HashSet<IGenericParameterProvider>();
        //    foreach(var provider in paramProviders)
        //    {
        //        switch(provider.GenericParameterType)
        //        {
        //            case GenericParameterType.Type:
        //                TypeReference trProvider = (TypeReference)provider;
        //                if(trProvider.IsGenericInstance)
        //                    providers.Add(trProvider.Resolve().GetElementType());
        //                else
        //                    providers.Add(trProvider);
        //                break;
        //            case GenericParameterType.Method:
        //                MethodReference mrProvider = (MethodReference)provider;
        //                if(mrProvider.IsGenericInstance)
        //                    providers.Add(mrProvider.Resolve().GetElementMethod());
        //                else
        //                    providers.Add(mrProvider);
        //                break;
        //        }
        //    }
        //    paramProviders = providers.ToArray();
        //}

        private IGenericParameterProvider GetElementProvider(IGenericParameterProvider provider)
        {
            TypeReference typeReferenceProvider;
            if((typeReferenceProvider = provider as TypeReference) != null)
                return typeReferenceProvider.Resolve().GetElementType();
            else
                return ((MethodReference)provider).GetElementMethod();
        }

        private TypeReference CreateTypeReference(TypeReference type, params IGenericParameterProvider[] paramProviders)
        {
            TypeReference declaringType = null;

            if(type.IsNested)
                declaringType = ReferenceType(type.DeclaringType, paramProviders);

            TypeReference reference = new TypeReference(type.Namespace, type.Name, module, type.Scope, type.IsValueType)
            {
                DeclaringType = declaringType,
            };

            CopyGenericParameters(type, reference);

            return reference;
        }

        private GenericInstanceType ReferenceGenericInstanceType(GenericInstanceType type, TypeReference typeReference, params IGenericParameterProvider[] paramProviders)
        {
            var genericInstance = new GenericInstanceType(typeReference);

            foreach(var argument in type.GenericArguments)
                genericInstance.GenericArguments.Add(ReferenceType(argument, paramProviders));

            return genericInstance;
        }

        private GenericInstanceMethod ReferenceGenericInstanceMethod(GenericInstanceMethod method, params IGenericParameterProvider[] paramProviders)
        {
            var elementMethod = ReferenceMethod(method.ElementMethod, paramProviders);
            var genericInstanceMethod = new GenericInstanceMethod(elementMethod);

            foreach(var argument in method.GenericArguments)
            {
                genericInstanceMethod.GenericArguments.Add(ReferenceType(argument, paramProviders));
            }

            return genericInstanceMethod;
        }

        private void CopyParameters(MethodReference sourceMethod, MethodReference targetMethod, params IGenericParameterProvider[] paramProviders)
        {
            if(!sourceMethod.HasParameters)
                return;

            var declaringType = targetMethod.DeclaringType;

            //to have full method signature when determining generic patameter owner
            foreach(var parameterDefinition in sourceMethod.Parameters)
            {
                targetMethod.Parameters.Add(parameterDefinition);
            }

            //fix references
            for(int i = 0; i < targetMethod.Parameters.Count; i++)
            {
                var parameterDefinition = targetMethod.Parameters[i];

                var parameterType = ReferenceType(parameterDefinition.ParameterType, paramProviders);
                ParameterDefinition newParameter = new ParameterDefinition(parameterDefinition.Name, parameterDefinition.Attributes, parameterType);

                targetMethod.Parameters[i] = newParameter;
            }
        }

        private void CopyGenericParameters(IGenericParameterProvider source, IGenericParameterProvider target)
        {
            if(!source.HasGenericParameters)
                return;

            foreach(var parameter in source.GenericParameters)
            {
                GenericParameter newParameter = new GenericParameter(parameter.Name, target)
                {
                    Attributes = parameter.Attributes
                };

                target.GenericParameters.Add(newParameter);

                foreach(var constraint in parameter.Constraints)
                {
                    newParameter.Constraints.Add(ReferenceType(constraint, target));
                }
            }
        }
    }
}
