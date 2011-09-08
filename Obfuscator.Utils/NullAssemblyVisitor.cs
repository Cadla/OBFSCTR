
namespace Obfuscator.Utils
{
    public class NullAssemblyVisitor : IAssemblyVisitor
    {
        public virtual VisitorLevel Level()
        {
            return VisitorLevel.MethodBodys;
        }

        public virtual void VisitAssemblyDefinition(Mono.Cecil.AssemblyDefinition assembly)
        {

        }

        public virtual void VisitModuleDefinition(Mono.Cecil.ModuleDefinition module)
        {

        }

        public virtual void VisitTypeDefinition(Mono.Cecil.TypeDefinition type)
        {

        }

        public virtual void VisitMethodDefinition(Mono.Cecil.MethodDefinition method)
        {

        }

        public virtual void VisitFieldDefinition(Mono.Cecil.FieldDefinition field)
        {

        }

        public virtual void VisitEventDefinition(Mono.Cecil.EventDefinition @event)
        {

        }

        public virtual void VisitPropertyDefinition(Mono.Cecil.PropertyDefinition property)
        {

        }

        public virtual void VisitParameterDefinition(Mono.Cecil.ParameterDefinition parameter)
        {

        }
        
        public virtual void VisitVariableDefinition(Mono.Cecil.Cil.VariableDefinition variable)
        {

        }

        public virtual void VisitModuleReference(Mono.Cecil.ModuleReference module)
        {

        }

        public virtual void VisitTypeReference(Mono.Cecil.TypeReference type)
        {

        }

        public virtual void VisitMethodReference(Mono.Cecil.MethodReference method)
        {

        }

        public virtual void VisitFieldReference(Mono.Cecil.FieldReference field)
        {

        }

        public virtual void VisitEventReference(Mono.Cecil.EventReference @event)
        {

        }

        public virtual void VisitPropertyReference(Mono.Cecil.PropertyReference property)
        {

        }

        public virtual void VisitParameterReference(Mono.Cecil.ParameterReference parameter)
        {

        }

        public virtual void VisitAssemblyReference(Mono.Cecil.AssemblyNameReference reference)
        {

        }

        public virtual void VisitMethodReturnType(Mono.Cecil.MethodReturnType returnType)
        {

        }

        public virtual void VisitInstruction(Mono.Cecil.Cil.Instruction instruction)
        {
            
        }

        public virtual void VisitGenericParameter(Mono.Cecil.GenericParameter genericParameter)
        {

        }

        public virtual void VisitCustomAttribute(Mono.Cecil.CustomAttribute attribute)
        {

        }

        public virtual void VisitSecurityDeclaration(Mono.Cecil.SecurityDeclaration securityDeclaration)
        {

        }

        public virtual void VisitResource(Mono.Cecil.Resource resource)
        {

        }
    }
}
