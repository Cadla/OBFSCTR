using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Obfuscator.Utils
{
    public sealed class AssemblyVisitor
    {
        private IAssemblyVisitor visitor_;

        readonly HashSet<IMetadataTokenProvider> visited = new HashSet<IMetadataTokenProvider>();

        private delegate void VisitElement<T>(T element);

        // TODO parseMethods doesn't make sense in this place. should be defined somewhere in the implementation of IAssemblyVisitor interface
        public AssemblyVisitor()
        {
        }

        public void ConductVisit(AssemblyDefinition assembly, IAssemblyVisitor visitor)
        {
            visitor_ = visitor;

            VisitAssemblyDefinition(assembly);
        }

        private void VisitAssemblyDefinition(AssemblyDefinition assembly)
        {
            //if (CheckVisited(assembly))
            //    return;

            visitor_.VisitAssemblyDefinition(assembly);

            VisitCollection(assembly.CustomAttributes, VisitCustomAttribute, () => assembly.HasCustomAttributes);

            VisitCollection(assembly.SecurityDeclarations, VisitSecurityDeclaration, () => assembly.HasSecurityDeclarations);

            VisitCollection(assembly.Modules, VisitModuleDefinition);
        }

        private void VisitModuleDefinition(ModuleDefinition module)
        {
            //if (CheckVisited(module))
            //    return;            

            visitor_.VisitModuleDefinition(module);

            VisitCollection(module.AssemblyReferences, VisitAssemblyReference, () => module.HasAssemblyReferences);

            VisitCollection(module.CustomAttributes, VisitCustomAttribute, () => module.HasCustomAttributes);

            // TODO check what .GetMemberReferences do. Maybe it doesn't make sense to go through all 
            // instructions and parse them as it may happpend that all references an assembly have are already accessible through this method

            // TODO what about Symbols (.HasSymbols .ReadSymbols())

            VisitCollection(module.Types, VisitTypeDefinition, () => module.HasTypes);

            VisitCollection(module.Resources, VisitResource, () => module.HasResources);
        }

        private void VisitTypeDefinition(TypeDefinition type)
        {
            if (type == null)
                return;

            // VisitTypeDefinition is invoked only once for each type
            //if (CheckVisited(type))
            //    return;

            visitor_.VisitTypeDefinition(type);

            // TODO check whether necessary
            //VisitTypeDefinition(type.DeclaringType);

            VisitTypeReference(type.BaseType);

            VisitCollection(type.CustomAttributes, VisitCustomAttribute, () => type.HasCustomAttributes);

            VisitCollection(type.SecurityDeclarations, VisitSecurityDeclaration, () => type.HasSecurityDeclarations);

            VisitCollection(type.GenericParameters, VisitGenericParameter, () => type.HasGenericParameters);

            VisitCollection(type.Interfaces, VisitTypeReference, () => type.HasInterfaces);

            VisitCollection(type.NestedTypes, VisitTypeDefinition, () => type.HasNestedTypes);

            VisitCollection(type.Fields, VisitFieldDefinition, () => type.HasFields);

            VisitCollection(type.Properties, VisitPropertyDefinition, () => type.HasProperties);

            VisitCollection(type.Events, VisitEventDefinition, () => type.HasEvents);

            VisitCollection(type.Methods, VisitMethodDefinition, () => type.HasMethods);
        }

        private void VisitTypeReference(TypeReference type)
        {
            if (visitor_.Level() == VisitorLevel.Definitions)
                return;

            if (type == null)
                return;

            if (CheckVisited(type))
                return;

            if (type.IsGenericParameter)
            {
                VisitGenericParameter(type as GenericParameter);
                return;
            }

            visitor_.VisitTypeReference(type);

            VisitCollection(type.GenericParameters, VisitGenericParameter, () => type.HasGenericParameters);

            // TODO check weather necessary
            VisitTypeReference(type.DeclaringType);
        }

        private void VisitFieldDefinition(FieldDefinition field)
        {
            //if (CheckVisited(field))
            //    return;

            visitor_.VisitFieldDefinition(field);

            // TODO is there a way to get here not from declaring type of this field?
            //     VisitTypeDefinition(field.DeclaringType);

            VisitTypeReference(field.FieldType);

            VisitCollection(field.CustomAttributes, VisitCustomAttribute, () => field.HasCustomAttributes);
        }

        private void VisitFieldReference(FieldReference field)
        {
            if (visitor_.Level() == VisitorLevel.Definitions)
                return;

            if (CheckVisited(field))
                return;

            visitor_.VisitFieldReference(field);

            VisitTypeReference(field.FieldType);
        }

        private void VisitPropertyDefinition(PropertyDefinition property)
        {
            //if (CheckVisited(property))
            //    return;

            visitor_.VisitPropertyDefinition(property);

            // TODO is there a way to get here not from declaring type of this field?
            //     VisitTypeDefinition(property.DeclaringType);

            VisitTypeReference(property.PropertyType);

            VisitCollection(property.CustomAttributes, VisitCustomAttribute, () => property.HasCustomAttributes);

            VisitMethodDefinition(property.GetMethod);

            VisitMethodDefinition(property.SetMethod);

            // NOTE property parameters - VB compability and also indexers in C#
            VisitCollection(property.Parameters, VisitParameterDefinition, () => property.HasParameters);

            // NOTE compatibility?
            VisitCollection(property.OtherMethods, VisitMethodDefinition, () => property.HasOtherMethods);
        }

        private void VisitEventDefinition(EventDefinition @event)
        {
            //if (CheckVisited(@event))
            //    return;
            visitor_.VisitEventDefinition(@event);

            // TODO is there a way to get here not from declaring type of this field?
            //       VisitTypeDefinition(@event.DeclaringType);

            VisitTypeReference(@event.EventType);

            VisitCollection(@event.CustomAttributes, VisitCustomAttribute, () => @event.HasCustomAttributes);

            VisitMethodDefinition(@event.AddMethod);

            VisitMethodDefinition(@event.RemoveMethod);

            // NOTE seems like raise method for and event can be overrided in c++, http://msdn.microsoft.com/en-us/library/5f3csfsa.aspx
            VisitMethodDefinition(@event.InvokeMethod);

            // NOTE compatibility again?
            VisitCollection(@event.OtherMethods, VisitMethodDefinition, () => @event.HasOtherMethods);
        }

        private void VisitMethodDefinition(MethodDefinition method)
        {
            if (method == null)
                return;

            //if (CheckVisited(method))
            //    return;

            visitor_.VisitMethodDefinition(method);

            // TODO is there a way to get here not from declaring type of this field?
            //     VisitTypeDefinition(method.DeclaringType);

            VisitTypeReference(method.ReturnType);

            VisitCollection(method.GenericParameters, VisitGenericParameter, () => method.HasGenericParameters);

            VisitCollection(method.Overrides, VisitMethodReference, () => method.HasOverrides);

            VisitCollection(method.Parameters, VisitParameterDefinition, () => method.HasParameters);

            // TODO method.HasSecurity - security attributes 

            VisitMethodReturnType(method.MethodReturnType);

            VisitCollection(method.CustomAttributes, VisitCustomAttribute, () => method.HasCustomAttributes);

            VisitCollection(method.SecurityDeclarations, VisitSecurityDeclaration, () => method.HasSecurityDeclarations);

            if (method.HasBody)
                VisitMethodBody(method.Body);
        }

        private void VisitMethodBody(MethodBody body)
        {
            if (visitor_.Level() != VisitorLevel.MethodBodys)
                return;

            visitor_.VisitMethodBody(body);

            VisitCollection(body.Variables, VisitVariable, () => body.HasVariables);

            foreach (var instruction in body.Instructions)
            {
                visitor_.VisitInstruction(instruction);

                object operand = instruction.Operand;

                if (operand is FieldReference)
                {
                    VisitFieldReference((FieldReference)operand);
                }
                else if (operand is MethodReference)
                {
                    VisitMethodReference((MethodReference)operand);
                }
                else if (operand is TypeReference)
                {
                    VisitTypeReference((TypeReference)operand);
                }
            }

            foreach (var handler in body.ExceptionHandlers)
                if (handler.CatchType != null)
                    VisitTypeReference(handler.CatchType);
        }

        private void VisitVariable(VariableDefinition variable)
        {
            // NOTE only mathers when referenced? 
            //if (CheckVisited(variable))
            //    return;

            visitor_.VisitVariableDefinition(variable);

            //VisitCollection(body.Variables, VisitVariable, () => b.HasVariables);

            // TODO ?

            VisitTypeReference(variable.VariableType);
        }

        private void VisitMethodReturnType(MethodReturnType returnType)
        {
            if (visitor_.Level() == VisitorLevel.Definitions)
                return;

            visitor_.VisitMethodReturnType(returnType);

            VisitCollection(returnType.CustomAttributes, VisitCustomAttribute, () => returnType.HasCustomAttributes);
        }

        private void VisitMethodReference(MethodReference method)
        {
            if (visitor_.Level() == VisitorLevel.Definitions)
                return;

            if (CheckVisited(method))
                return;

            visitor_.VisitMethodReference(method);

            // TODO is there a way to get here not from declaring type of this field?
            VisitTypeReference(method.DeclaringType);

            VisitTypeReference(method.ReturnType);

            VisitCollection(method.GenericParameters, VisitGenericParameter, () => method.HasGenericParameters);

            VisitCollection(method.Parameters, VisitParameterDefinition, () => method.HasParameters);

            // TODO method.HasSecurity - security attributes 

            VisitMethodReturnType(method.MethodReturnType);
        }

        private void VisitParameterDefinition(ParameterDefinition parameter)
        {
            if (CheckVisited(parameter))
                return;

            visitor_.VisitParameterDefinition(parameter);

            VisitCollection(parameter.CustomAttributes, VisitCustomAttribute, () => parameter.HasCustomAttributes);

            VisitTypeReference(parameter.ParameterType);
        }

        private void VisitAssemblyReference(AssemblyNameReference reference)
        {
            if (visitor_.Level() == VisitorLevel.Definitions)
                return;

            if (CheckVisited(reference))
                return;

            visitor_.VisitAssemblyReference(reference);
        }

        private void VisitGenericParameter(GenericParameter parameter)
        {
            if (CheckVisited(parameter))
                return;

            visitor_.VisitGenericParameter(parameter);

            VisitCollection(parameter.CustomAttributes, VisitCustomAttribute, () => parameter.HasCustomAttributes);

            VisitTypeReference(parameter.DeclaringType);

            VisitCollection(parameter.GenericParameters, VisitGenericParameter, () => parameter.HasGenericParameters);

            VisitCollection(parameter.Constraints, VisitTypeReference, () => parameter.HasConstraints);

            if (parameter.Owner is TypeReference)
                VisitTypeReference((TypeReference)parameter.Owner);
            else
                VisitMethodReference((MethodReference)parameter.Owner);
        }

        private void VisitCustomAttribute(CustomAttribute attribute)
        {
            visitor_.VisitCustomAttribute(attribute);
            // TODO not finished
        }

        private void VisitSecurityDeclaration(SecurityDeclaration securityDeclaration)
        {
            visitor_.VisitSecurityDeclaration(securityDeclaration);
            // TODO not finished
        }

        private void VisitResource(Resource resource)
        {
            visitor_.VisitResource(resource);
        }

        private bool CheckVisited(IMetadataTokenProvider provider)
        {
            if (visited.Contains(provider))
                return true;

            visited.Add(provider);
            return false;
        }

        private void VisitCollection<T>(Collection<T> collection, VisitElement<T> method, Func<bool> condition)
        {
            if (condition())
            {
                VisitCollection(collection, method);
            }
        }

        private void VisitCollection<T>(Collection<T> collection, VisitElement<T> method)
        {
            foreach (var t in collection)
            {
                method(t);
            }
        }
    }
}
