//////
////// TypeMapStep.cs
//////
////// Author:
//////   Jb Evain (jbevain@novell.com)
//////
////// (C) 2009 Novell, Inc.
//////
////// Permission is hereby granted, free of charge, to any person obtaining
////// a copy of this software and associated documentation files (the
////// "Software"), to deal in the Software without restriction, including
////// without limitation the rights to use, copy, modify, merge, publish,
////// distribute, sublicense, and/or sell copies of the Software, and to
////// permit persons to whom the Software is furnished to do so, subject to
////// the following conditions:
//////
////// The above copyright notice and this permission notice shall be
////// included in all copies or substantial portions of the Software.
//////
////// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
////// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
////// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
////// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
////// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
////// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
////// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//////

////using System;
////using System.Linq;
////using System.Collections;

////using Mono.Cecil;
////using Mono.Cecil.Rocks;
////using System.Collections.Generic;
////using Mono.Cecil.Cil;
////using Obfuscator.Utils;
////using Obfuscator.MetadataBuilder.Extensions;
////using System.Diagnostics;

////namespace Obfuscator.Steps.Renaming {

////    public class TypeMapStep : BaseStep {

////        Dictionary<MethodDefinition, List<MethodReference>> _methodOverrides;
////        ReferenceResolver _resolver;

////        public TypeMapStep()
////        {
////            _methodOverrides = new Dictionary<MethodDefinition, List<MethodReference>>();
            
////        }

////        protected override void ProcessAssembly (AssemblyDefinition assembly)
////        {

////            foreach (TypeDefinition type in assembly.MainModule.Types)
////            {
////                _resolver = ReferenceResolver.GetDefaultResolver(assembly.MainModule);
////                MapType(type);
////            }
////        }


////        protected override void EndProcess()
////        {
////            foreach (var key in _methodOverrides)
////            {
////                var implementation = key.Key;
////                foreach (var method in key.Value)
////                {
////                    var reference = method.Module.Import(method);
////                    if (!implementation.Overrides.Contains(reference))
////                        implementation.Overrides.Add(reference);
////                }
////            }
////        }
        
////        void MapType (TypeDefinition type)
////        {
////            MapVirtualMethods (type);
////            MapInterfaceMethodsInTypeHierarchy (type);

////            if (!type.HasNestedTypes)
////                return;

////            foreach (var nested in type.NestedTypes)
////                MapType (nested);
////        }

////        void MapInterfaceMethodsInTypeHierarchy (TypeDefinition type)
////        {
////            if (!type.HasInterfaces)
////                return;

////            foreach (TypeReference @interface in type.Interfaces) {
////                var iface = @interface.Resolve ();
////                if (iface == null || !iface.HasMethods)
////                    continue;

////                foreach (MethodDefinition method in iface.Methods) {
////                    MethodDefinition implementation;
                    
////                    if (type.Methods.Any(m => Helper.IsExplicitImplementation(method, m)))
////                        // does not need to ad method to overrides since it's already there
////                        continue;

////                    if ((implementation = TryMatchMethod(type, method)) != null)
////                    {                        
////                         AddOverride(method, iface, implementation);                     
////                        continue;
////                    }
                    					
////                    var @override = CreateExplicitInterfaceImplementation(type, method);
////                    AddOverride(method, iface, @override);					
////                }
////            }
////        }

////        void MapVirtualMethods (TypeDefinition type)
////        {
////            if (!type.HasMethods)
////                return;

////            foreach (MethodDefinition method in type.Methods) {
////                if (!method.IsVirtual)
////                    continue;

////                MapVirtualMethod (method);

////                if (method.HasOverrides)
////                    MapOverrides (method);
////            }
////        }

////        void MapVirtualMethod (MethodDefinition method)
////        {
////            MapVirtualBaseMethod (method);
////    //		MapVirtualInterfaceMethod (method);
////        }

////        void MapVirtualBaseMethod (MethodDefinition method)
////        {
////            TypeDefinition @base = GetBaseType(method.DeclaringType);
////            while (@base != null)
////            {
////                MethodDefinition base_method = TryMatchMethod(@base, method);
////                if (base_method != null)
////                {
////                    AddOverride(base_method, @base, method);
////                    break;
////                }
////                @base = GetBaseType(@base);
////            }
////        }

////        //void MapVirtualInterfaceMethod (MethodDefinition method)
////        //{
////        //    MethodReference @base = GetBaseMethodInInterfaceHierarchy (method);
////        //    if (@base == null)
////        //        return;

////        //    AddOverride(@base, method);
////        //}

////        void MapOverrides (MethodDefinition method)
////        {
////            foreach (MethodReference override_ref in method.Overrides) {
////                MethodDefinition @override = override_ref.Resolve ();
////                if (@override == null)
////                    continue;

//////				AnnotateMethods (@override, method);
////            }
////        }


////        MethodReference GetBaseMethodInTypeHierarchy(MethodDefinition method)
////        {
////            return GetBaseMethodInTypeHierarchy(method.DeclaringType, method);
////        }

////        MethodReference GetBaseMethodInTypeHierarchy(TypeDefinition type, MethodDefinition method)
////        {
////            TypeDefinition @base = GetBaseType(type);
////            while (@base != null)
////            {
////                MethodDefinition base_method = TryMatchMethod(@base, method);
////                if (base_method != null)
////                    return base_method;
////                @base = GetBaseType(@base);
////            }
////            return null;
////        }

////        //MethodReference GetBaseMethodInInterfaceHierarchy(MethodDefinition method)
////        //{
////        //    return GetBaseMethodInInterfaceHierarchy (method.DeclaringType, method);
////        //}

////        //MethodReference GetBaseMethodInInterfaceHierarchy(TypeDefinition type, MethodDefinition method)
////        //{
////        //    if (!type.HasInterfaces)
////        //        return null;

////        //    foreach (TypeReference interface_ref in type.Interfaces) {
////        //        TypeDefinition @interface = interface_ref.Resolve ();
////        //        if (@interface == null)
////        //            continue;

////        //        MethodReference base_method = TryMatchMethod (@interface, method);
////        //        if (base_method != null)
////        //                return base_method;

////        //        base_method = GetBaseMethodInInterfaceHierarchy (@interface, method);
////        //        if (base_method != null)
////        //            return base_method;
////        //    }

////        //    return null;
////        //}

////        static MethodDefinition TryMatchMethod (TypeDefinition type, MethodDefinition method)
////        {
////            if (!type.HasMethods)
////                return null;

////            foreach (MethodDefinition candidate in type.Methods)
////                if (MethodMatch (candidate, method))
////                    return candidate;

////            return null;
////        }

//        static bool MethodMatch (MethodDefinition candidate, MethodDefinition method)
//        {
//            if (!candidate.IsVirtual)
//                return false;

//            if (candidate.Name != method.Name)
//                return false;

//            if (!TypeMatch (candidate.ReturnType, method.ReturnType))
//                return false;

//            if (candidate.Parameters.Count != method.Parameters.Count)
//                return false;

//            for (int i = 0; i < candidate.Parameters.Count; i++)
//                if (!TypeMatch (candidate.Parameters [i].ParameterType, method.Parameters [i].ParameterType))
//                    return false;

//            return true;
//        }

//        static bool TypeMatch (IModifierType a, IModifierType b)
//        {
//            if (!TypeMatch (a.ModifierType, b.ModifierType))
//                return false;

//            return TypeMatch (a.ElementType, b.ElementType);
//        }

//        static bool TypeMatch (TypeSpecification a, TypeSpecification b)
//        {
//            if (a is GenericInstanceType)
//                return TypeMatch ((GenericInstanceType) a, (GenericInstanceType) b);

//            if (a is IModifierType)
//                return TypeMatch ((IModifierType) a, (IModifierType) b);

//            return TypeMatch (a.ElementType, b.ElementType);
//        }

//        static bool TypeMatch (GenericInstanceType a, GenericInstanceType b)
//        {
//            if (!TypeMatch (a.ElementType, b.ElementType))
//                return false;

//            if (a.GenericArguments.Count != b.GenericArguments.Count)
//                return false;

//            if (a.GenericArguments.Count == 0)
//                return true;

//            for (int i = 0; i < a.GenericArguments.Count; i++)
//                if (!TypeMatch (a.GenericArguments [i], b.GenericArguments [i]))
//                    return false;

//            return true;
//        }

//        static bool TypeMatch (TypeReference a, TypeReference b)
//        {
//            if (a is GenericParameter)
//                return true;

//            if (a is TypeSpecification || b is TypeSpecification) {
//                if (a.GetType () != b.GetType ())
//                    return false;

//                return TypeMatch ((TypeSpecification) a, (TypeSpecification) b);
//            }

//            return a.FullName == b.FullName;
//        }

//        static TypeDefinition GetBaseType (TypeDefinition type)
//        {
//            if (type == null || type.BaseType == null)
//                return null;

//            return type.BaseType.Resolve ();
//        }

////        private void AddOverride(MethodDefinition @base, TypeDefinition baseDeclaringType, MethodDefinition implementation)
////        {
////            var base_reference = _resolver.ReferenceMethod(@base);
////            if (baseDeclaringType.IsGenericInstance)
////                base_reference = MakeGeneric(base_reference, baseDeclaringType);

////            if (_methodOverrides.ContainsKey(implementation))
////                _methodOverrides[implementation].Add(base_reference);
////            else
////                _methodOverrides.Add(implementation, new List<MethodReference>() { base_reference });
////        }

////        public static MethodReference MakeGeneric(MethodReference self, TypeReference declaringType)
////        {
////            var reference = new MethodReference(self.Name, self.ReturnType)
////            {                
////                DeclaringType = declaringType,
////                HasThis = self.HasThis,
////                ExplicitThis = self.ExplicitThis,             
////                CallingConvention = MethodCallingConvention.Generic,
////            };
////            foreach (var parameter in self.Parameters)
////                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
////            foreach (var generic_parameter in self.GenericParameters)
////                reference.GenericParameters.Add(new GenericParameter(reference));
////            return reference;
////        } 

////        private MethodDefinition CreateExplicitInterfaceImplementation(TypeDefinition type, MethodDefinition method)
////        {
////            var customMethod = type.InjectMethod(method, ReferenceResolver.GetDefaultResolver(type.Module), false);

////            customMethod.Attributes = MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final |
////                MethodAttributes.HideBySig | MethodAttributes.NewSlot;


////            var body = customMethod.Body = new MethodBody(customMethod);
////            body.InitLocals = true;
////            var returnType = type.Module.Import(method.ReturnType);
////            body.Variables.Add(new VariableDefinition(returnType));
////            var processor = customMethod.Body.GetILProcessor();
////            processor.Emit(OpCodes.Nop);

////            for (byte i = 0; i < method.Parameters.Count + 1; ++i)
////            {
////                switch (i)
////                {
////                    case 0:
////                        processor.Emit(OpCodes.Ldarg_0);
////                        break;
////                    case 1:
////                        processor.Emit(OpCodes.Ldarg_1);
////                        break;
////                    case 2:
////                        processor.Emit(OpCodes.Ldarg_2);
////                        break;
////                    case 3:
////                        processor.Emit(OpCodes.Ldarg_3);
////                        break;
////                    default:
////                        processor.Emit(OpCodes.Ldarg_S, i);
////                        break;

////                }
////            }
            
////            var @base = GetBaseMethodInTypeHierarchy(type, method);
////            Debug.Assert(@base != null);
////            processor.Emit(OpCodes.Call, @base);
////            processor.Emit(OpCodes.Stloc_0);
////            var ldloc = processor.Create(OpCodes.Ldloc_0);
////            processor.Append(ldloc);
////            var brs = processor.Create(OpCodes.Br_S, ldloc);
////            processor.InsertBefore(ldloc, brs);
////            processor.Emit(OpCodes.Ret);

////            return customMethod;
////        }
////    }
////}
