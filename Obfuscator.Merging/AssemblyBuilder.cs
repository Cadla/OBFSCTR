using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Obfuscator;

namespace Merger.Builders
{
    public class AssemblyBuilder
    {        
        ModuleDefinition module;
        ReferenceResolver resolver;
        TypeBuilder typeBuilder;

        public AssemblyDefinition Assembly
        {
            get;
            private set;
        }

        public AssemblyBuilder(AssemblyDefinition assembly, ReferenceResolver resolver)
        {
            if(assembly == null)
                throw new ArgumentNullException("assembly");
            if(resolver == null)
                throw new ArgumentNullException("resolver");

            Assembly = CloneAssembly(assembly);            
            
            this.resolver = resolver;

            resolver.Module = Assembly.MainModule;
            typeBuilder = new TypeBuilder(Assembly.MainModule, resolver);
        }

        public AssemblyDefinition CloneAssembly(AssemblyDefinition assembly)
        {
            var newAssembly = AssemblyDefinition.CreateAssembly(assembly.Name, assembly.MainModule.Name, new ModuleParameters()
            {
                Kind = assembly.MainModule.Kind,
                Architecture = assembly.MainModule.Architecture,
                Runtime = assembly.MainModule.Runtime,
                AssemblyResolver = assembly.MainModule.AssemblyResolver
            });

            return newAssembly;
        }

        public void InjectAssemblyContent(AssemblyDefinition source)
        {
            CopyCustomAttributes(source, Assembly);

            CopyCustomAttributes(source.MainModule, module);

            CopyResources(module, Assembly.MainModule);

            CopySecurityDeclarations(source, Assembly);

            foreach(var type in source.MainModule.Types.ToList())
            {
                if(type.Name != "<Module>")
                {
                    InjectType(type, false);
                }
            }
        }
             
        public CustomAttribute InjectCustomAttribute(Mono.Cecil.ICustomAttributeProvider target, CustomAttribute attribute)
        {                    
            if(module == null)
                throw new ArgumentNullException("module");
            if(target == null)
                throw new ArgumentNullException("target");
            if(attribute == null)
                throw new ArgumentNullException("attribute");

            TypeReference attributeType = ReferenceOrInjectType(attribute.AttributeType);

            // no context required as attributes cannot be generic
            MethodReference constructor = ReferenceOrInjectMethod(attribute.Constructor);

            CustomAttribute newAttribute;

            if((newAttribute = Helper.GetCustomAttribute(target.CustomAttributes, attribute)) != null)
                return newAttribute;

            newAttribute = new CustomAttribute(constructor);//, attr.GetBlob());

            target.CustomAttributes.Add(newAttribute);

            CopyCustomAttributeArguments(attribute.ConstructorArguments, newAttribute.ConstructorArguments);

            CopyCustomAttributeNamedArguments(attribute.Fields, newAttribute.Fields);

            CopyCustomAttributeNamedArguments(attribute.Properties, newAttribute.Properties);

            return newAttribute;
        }
  
     

        //private void CopyCustomAttributes(Mono.Cecil.ICustomAttributeProvider source, Mono.Cecil.ICustomAttributeProvider target)
        //{
        //    if(!source.HasCustomAttributes)
        //        return;

        //    foreach(var attribute in source.CustomAttributes)
        //    {
        //        InjectCustomAttribute(target, attribute);
        //    }
        //}

        ////TODO duplicated code in below two
        //private void CopyCustomAttributeNamedArguments(Collection<Mono.Cecil.CustomAttributeNamedArgument> source, Collection<Mono.Cecil.CustomAttributeNamedArgument> target)
        //{
        //    foreach(var namedArgument in source)
        //    {
        //        var argumentType = ReferenceOrInjectType(namedArgument.Argument.Type);
        //        CustomAttributeArgument argument = new CustomAttributeArgument(argumentType, namedArgument.Argument.Value);

        //        target.Add(new Mono.Cecil.CustomAttributeNamedArgument(namedArgument.Name, argument));
        //    }
        //}

        //private void CopyCustomAttributeArguments(Collection<Mono.Cecil.CustomAttributeArgument> source, Collection<Mono.Cecil.CustomAttributeArgument> target)
        //{
        //    foreach(var argument in source)
        //    {
        //        var argumentType = ReferenceOrInjectType(argument.Type);
        //        target.Add(new CustomAttributeArgument(argumentType, argument.Value));
        //    }
        //}

        //private void CopyResources(ModuleDefinition source, ModuleDefinition target)
        //{
        //    if(!source.HasResources)
        //        return;

        //    foreach(var resource in source.Resources)
        //    {
        //        if(!target.Resources.Contains(resource))
        //            target.Resources.Add(resource);
        //    }
        //}

        //private void CopyGenericParameters(IGenericParameterProvider source, IGenericParameterProvider target)
        //{
        //    if(!source.HasGenericParameters)
        //        return;

        //    foreach(var parameter in source.GenericParameters)
        //    {
        //        CopyGenericParameter(parameter, target);
        //    }
        //}

        //private void CopyGenericParameter(GenericParameter parameter, IGenericParameterProvider owner)
        //{
        //    GenericParameter newParameter = new GenericParameter(parameter.Name, owner)
        //    {
        //        Attributes = parameter.Attributes
        //    };

        //    owner.GenericParameters.Add(newParameter);

        //    CopyCustomAttributes(newParameter, parameter);

        //    foreach(var constraint in parameter.Constraints)
        //    {
        //        if(owner is MethodReference)
        //            newParameter.Constraints.Add(ReferenceOrInjectType(constraint, owner, ((MethodReference)owner).DeclaringType));
        //        else
        //            newParameter.Constraints.Add(ReferenceOrInjectType(constraint, owner));
        //    }
        //}

        //private void CopySecurityDeclarations(ISecurityDeclarationProvider source, ISecurityDeclarationProvider target)
        //{
        //    if(!source.HasSecurityDeclarations)
        //        return;

        //    foreach(var declaration in source.SecurityDeclarations)
        //    {
        //        var newDeclaration = new SecurityDeclaration(declaration.Action);

        //        CopySecurityAttributes(declaration, newDeclaration);

        //        target.SecurityDeclarations.Add(newDeclaration);
        //    }
        //}

        //private void CopySecurityAttributes(SecurityDeclaration source, SecurityDeclaration target)
        //{
        //    if(!source.HasSecurityAttributes)
        //        return;

        //    foreach(var attribute in source.SecurityAttributes)
        //    {
        //        var attributeType = ReferenceOrInjectType(attribute.AttributeType);
        //        var newAttribute = new SecurityAttribute(attributeType);

        //        CopyCustomAttributeNamedArguments(attribute.Fields, newAttribute.Fields);

        //        CopyCustomAttributeNamedArguments(attribute.Properties, newAttribute.Properties);

        //        target.SecurityAttributes.Add(newAttribute);
        //    }
        //}



    }
}
