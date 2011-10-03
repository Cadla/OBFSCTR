using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Obfuscator.MetadataBuilder;
using Obfuscator.Utils;
using Obfuscator.Common.Steps;
using Obfuscator.Renaming.Steps;
using Obfuscator.Renaming;

namespace Obfuscator.Renaming.Steps
{
    //TODO: Refactor
    internal class FillMethodImplTablesStep : RenamingBaseStep
    {
        ReferenceResolver _resolver;
        Dictionary<MethodDefinition, List<MethodReference>> _methodOverrides = new Dictionary<MethodDefinition, List<MethodReference>>();

        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            _resolver = ReferenceResolver.GetDefaultResolver(assembly.MainModule);

            foreach (TypeDefinition type in assembly.MainModule.GetTypes().Where(t => !t.IsInterface))
            {
                FindImplementationsOfBaseMethods(type);
                FindImplementationsOfInterfaceMethods(type);
            }
        }

        protected override void EndProcess()
        {
            foreach (var key in _methodOverrides)
            {
                var implementation = key.Key;
                foreach (var method in key.Value)
                {
                    Debug.Assert(method.Module == implementation.Module);
                        
                    if(!implementation.Overrides.Contains(method))
                        implementation.Overrides.Add(method);
                }
            }
        }

        private void FindImplementationsOfInterfaceMethods(TypeDefinition type)
        {
            foreach (var @interface in type.Interfaces)
            {       
                var definition = @interface.Resolve();
                foreach (var method in definition.Methods)
                {
                    MethodReference interfaceMethod = method;
                    MethodReference replacedParameters = method;
                    if (@interface.IsGenericInstance)
                    {
                        var genericInstance = (GenericInstanceType)_resolver.ReferenceType(@interface, type);
                        interfaceMethod = MakeGeneric(method, genericInstance);

                        replacedParameters = ReplaceGenericParameters(interfaceMethod, genericInstance);
                    }
                                        
                    // Interface method is explictly implemented by the type. No need to add it to Overrides since it's already there    
                    MethodDefinition implementation;
                    if (GetExplicitInterfaceMethodImplementation(type, replacedParameters) != null)
                        continue;

                    implementation = GetImplicitInterfaceMethodImplementation(type, replacedParameters);

                    if (implementation != null)
                    {
                        AddOverride(implementation, interfaceMethod);
                        continue;
                    }

                    bool test = type.Methods.Any(m => m.Name.EndsWith(interfaceMethod.Name));

                    var customMethod = CreateCustomMethod(type, method, interfaceMethod);
                    AddOverride(customMethod, interfaceMethod);
                }
            }
        }

        private MethodDefinition CreateCustomMethod(TypeDefinition type, MethodDefinition method, MethodReference interfaceMethod)
        {
            var customMethod = CreateExplicitInterfaceImplementation(type, method);

            var baseMethod = GetBaseMethodReference(type, interfaceMethod);
            if (baseMethod == null)
                baseMethod = GetExplicitInterfaceMethodImplementationInBaseType(type, interfaceMethod);
            baseMethod = type.Module.Import(baseMethod);
            Debug.Assert(baseMethod != null);

            AddBaseMethodCall(customMethod, baseMethod);
            return customMethod;
        }
     
        private void FindImplementationsOfBaseMethods(TypeDefinition type)
        {
            if (type == null || !type.HasMethods)
                return;
            
            foreach (var method in type.Methods)
            {
                if (!method.IsVirtual || method.IsNewSlot)
                    continue;

                var baseMethod = GetBaseMethodReference(type, method);
                if (baseMethod != null)
                    AddOverride(method, baseMethod);
            }
        }

        private MethodReference GetBaseMethodReference(TypeDefinition type, MethodReference method)
        {
            var baseTypeReference = type.BaseType;
            var baseTypeDefinition = Helper.GetBaseTypeDefinition(type);
            while (baseTypeDefinition != null)
            {
                foreach (var candidate in baseTypeDefinition.Methods)
                {
                    MethodReference candidateReference = candidate;
                    MethodReference replaceParameters = candidate;
                    if (baseTypeReference.IsGenericInstance)
                    {
                        var genericInstance = (GenericInstanceType)_resolver.ReferenceType(baseTypeReference);
                        candidateReference = MakeGeneric(candidateReference, genericInstance);

                        replaceParameters = ReplaceGenericParameters(candidateReference, genericInstance);
                    }

                    if (Helper.HaveSameSignature(replaceParameters, method))                    
                        return candidateReference;                        
                }
                baseTypeReference = baseTypeDefinition.BaseType;
                baseTypeDefinition = Helper.GetBaseTypeDefinition(baseTypeDefinition);
            }
            return null;
        }
        
        private void AddOverride(MethodDefinition implementation, MethodReference @base)
        {
            var base_reference = @base;
            if(@base.Module != implementation.Module)
                base_reference = _resolver.ReferenceMethod(@base); 
            if (_methodOverrides.ContainsKey(implementation))
                _methodOverrides[implementation].Add(base_reference);
            else
                _methodOverrides.Add(implementation, new List<MethodReference>() { base_reference });
        }

        private static MethodDefinition GetExplicitInterfaceMethodImplementationInBaseType(TypeDefinition type, MethodReference interfaceMethod)
        {
            var baseType = Helper.GetBaseTypeDefinition(type);
            while (baseType != null)
            {
                var implementation = GetExplicitInterfaceMethodImplementation(baseType, interfaceMethod);
                if (implementation != null)
                    return implementation;
                
                baseType = Helper.GetBaseTypeDefinition(baseType);
            }
            return null;
        }

        private static MethodDefinition GetExplicitInterfaceMethodImplementation(TypeDefinition type, MethodReference interfaceMethod)
        {
            MethodDefinition implementation = null;
            foreach (var candidate in type.Methods)
            {
                if (!candidate.HasOverrides)
                    continue;

                foreach (var @override in candidate.Overrides)
                {
                    if (Helper.HaveSameSignature(@override, interfaceMethod) && Helper.AreSame(@override.DeclaringType, interfaceMethod.DeclaringType))                    
                    {
                        Debug.Assert(implementation == null);
                        implementation = candidate;
                    }
                }
            }
            return implementation;
        }

        private static MethodDefinition GetImplicitInterfaceMethodImplementation(TypeDefinition type, MethodReference interfaceMethod)
        {
            MethodDefinition implementation = null;
            foreach (var candidate in type.Methods)
            {
                if (!candidate.IsPublic || !candidate.IsVirtual)
                    continue;
                
                if (Helper.HaveSameSignature(candidate, interfaceMethod))
                {
                    Debug.Assert(implementation == null);
                    implementation = candidate;
                }
            }
            return implementation;
        }

        //TODO: move to helper
        private  MethodReference MakeGeneric(MethodReference method, GenericInstanceType declaringType)
        {   
            var reference = new MethodReference(method.Name, method.ReturnType )
            {
                DeclaringType = declaringType,
                HasThis = method.HasThis,
                ExplicitThis = method.ExplicitThis,
                CallingConvention = method.CallingConvention,
            };
            foreach (var parameter in method.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var generic_parameter in method.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            reference.ReturnType = _resolver.ReferenceType(reference.ReturnType, reference, declaringType);  
            return reference;
        }

        //TODO: move to helper
        private MethodReference ReplaceGenericParameters(MethodReference method, GenericInstanceType declaringType)
        {
            var reference = new MethodReference(method.Name, method.ReturnType)
            {
                DeclaringType = declaringType,
                HasThis = method.HasThis,
                ExplicitThis = method.ExplicitThis,
                CallingConvention = method.CallingConvention,
            };
            foreach (var parameter in method.Parameters)
            {
                var parameterType = parameter.ParameterType;
                if (parameter.ParameterType.IsGenericParameter)
                {
                    parameterType = declaringType.GenericArguments[((GenericParameter)parameter.ParameterType).Position];                   
                }
                reference.Parameters.Add(new ParameterDefinition(parameterType));
            }
            if (reference.ReturnType.IsGenericParameter)
            {
                reference.ReturnType = declaringType.GenericArguments[((GenericParameter)reference.ReturnType).Position];      
            }
            return reference;
        }

        private MethodDefinition CreateExplicitInterfaceImplementation(TypeDefinition type, MethodDefinition method)
        {
            var customMethod = type.InjectMethod(method, _resolver, false);

            customMethod.Attributes = MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot;

            return customMethod;
        }

        private  void AddBaseMethodCall(MethodDefinition method, MethodReference baseMethod)
        {
            var body = method.Body = new MethodBody(method);            

            var returnType = _resolver.ReferenceType(method.ReturnType);            
            bool isVoid = returnType.FullName == "System.Void";
            if (!isVoid)
            {             
                body.Variables.Add(new VariableDefinition(returnType));
            }

            var processor = method.Body.GetILProcessor();
            processor.Emit(OpCodes.Nop);

            var argCount = method.Parameters.Count + 1;
            for (byte i = 0; i < argCount; ++i)
            {
                switch (i)
                {
                    case 0:
                        processor.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        processor.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        processor.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        processor.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        processor.Emit(OpCodes.Ldarg_S, i);
                        break;
                }
            }


            processor.Emit(OpCodes.Tail);
            processor.Emit(OpCodes.Call, baseMethod);        
            processor.Emit(OpCodes.Ret);            
        }

        //public static MethodReference MakeGenericMethod(MethodReference self, params TypeReference[] arguments)
        //{
        //    if (self.GenericParameters.Count != arguments.Length)
        //        throw new ArgumentException();

        //    var instance = new GenericInstanceMethod(self);
        //    foreach (var argument in arguments)
        //        instance.GenericArguments.Add(argument);

        //    return instance;
        //}
        
    }
}
