using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Obfuscator.Utils
{    
    public enum VisitorLevel
    {
        Definitions,
        References,
        MethodBodys        
    }

    public interface IAssemblyVisitor
    {
        VisitorLevel Level();

        void VisitAssemblyDefinition(AssemblyDefinition assembly);

        void VisitModuleDefinition(ModuleDefinition module);
        void VisitTypeDefinition(TypeDefinition type);
        void VisitMethodDefinition(MethodDefinition method);
        void VisitFieldDefinition(FieldDefinition field);
        void VisitEventDefinition(EventDefinition @event);
        void VisitPropertyDefinition(PropertyDefinition property);
        void VisitParameterDefinition(ParameterDefinition parameter);
        void VisitVariableDefinition(VariableDefinition variable);

        void VisitModuleReference(ModuleReference module);
        void VisitTypeReference(TypeReference type);
        void VisitMethodReference(MethodReference method);
        void VisitFieldReference(FieldReference field);
        void VisitEventReference(EventReference @event);
        void VisitPropertyReference(PropertyReference property);
        void VisitParameterReference(ParameterReference parameter);

        void VisitAssemblyReference(AssemblyNameReference reference);

        void VisitMethodReturnType(MethodReturnType returnType);
        void VisitMethodBody(MethodBody body);
        void VisitInstruction(Instruction instruction);
        void VisitGenericParameter(GenericParameter genericParameter);
        void VisitCustomAttribute(CustomAttribute attribute);
        void VisitSecurityDeclaration(SecurityDeclaration securityDeclaration);

        void VisitResource(Resource resource);

        
    }
}

