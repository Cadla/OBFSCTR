using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Utils;
using Mono.Cecil;

namespace Obfuscator.OverridesResolver
{
    public class Visitor : NullAssemblyVisitor
    {
        Dictionary<TypeDefinition, List<MethodReference>> types = new Dictionary<TypeDefinition, List<MethodReference>>();
        Dictionary<MethodDefinition, List<MethodReference>> methodOverrides = new Dictionary<MethodDefinition, List<MethodReference>>();

        public void printDictionary()
        {
            //foreach (var t in types)
            //{
            //    Console.WriteLine(t.Key.Name);
            //    foreach (var m in t.Value)
            //    {
            //        Console.WriteLine("\t {0} from type {1}", m.Name, m.DeclaringType);
            //    }
            //}
            foreach (var t in methodOverrides)
            {
                Console.WriteLine("Method {0} in type {1}", t.Key.Name, t.Key.DeclaringType);
                foreach (var m in t.Value)
                {
                    Console.WriteLine("\t {0} from type {1}", m.Name, m.DeclaringType);
                    t.Key.Overrides.Add(m);
                }
            }

        }

        public override void VisitTypeDefinition(TypeDefinition type)
        {
            var interfaceMethods = new List<MethodDefinition>();

            foreach (var iface in type.Interfaces)
            {
                GetInterfaceMethods(iface, ref interfaceMethods);
            }

            foreach (var method in interfaceMethods)
            {
                bool test = true;
                var baseType = type.BaseType != null ? type.BaseType.Resolve() : null;
                do
                {
                    MethodDefinition overide;
                    if ((overide = type.Methods.SingleOrDefault(m => IsExplicitImplementation(method, m))) != null)
                    {
                        test = false;
                    //    AddOverride(overide, method); // it's already there
                        //if (methodOverrides.ContainsKey(overide))
                        //    methodOverrides[overide].Add(method);
                        //else
                        //    methodOverrides.Add(overide, new List<MethodDefinition>() { method });
                        break;
                    }
                    if ((overide = type.Methods.SingleOrDefault(m => IsImplicitImplementation(method, m))) != null)
                    {
                        test = false;
                        AddOverride(overide, method);
                        //if (methodOverrides.ContainsKey(overide))
                        //    methodOverrides[overide].Add(method);
                        //else
                        //    methodOverrides.Add(overide, new List<MethodDefinition>() { method });
                        break;
                    }
                    baseType = baseType.BaseType != null ? baseType.BaseType.Resolve() : null;
                } while (baseType != null);

                if (test == true)
                    throw new Exception();
            }

            //var inheritedMethods = new List<MethodDefinition>();

            //GetInheritedMethods(type, ref inheritedMethods);

            //types[type] = new List<MethodReference>();
            //types[type].AddRange(interfaceMethods);
            //types[type].AddRange(inheritedMethods);



            base.VisitTypeDefinition(type);
        }

        private static bool IsExplicitImplementation(MethodDefinition interfaceMethod, MethodDefinition m)
        {
            if(!(m.IsPrivate && m.IsVirtual && m.IsNewSlot && m.IsFinal))
                return false;

            if (interfaceMethod.DeclaringType + "." + interfaceMethod.Name != m.Name)
                return false;
            if (interfaceMethod.Parameters.Count != m.Parameters.Count)
                return false;

            var parameters = interfaceMethod.Parameters.Select(p => p.ParameterType).Except(m.Parameters.Select(p => p.ParameterType));
            return parameters.Count() == 0;            
        }

        private static bool IsImplicitImplementation(MethodDefinition method, MethodDefinition m)
        {
            return m.IsPublic && m.IsVirtual && m.IsNewSlot && HaveSameSignature(m, method);
        }

        private static void GetInterfaceMethods(TypeReference iface, ref List<MethodDefinition> result)
        {
            var resolved = iface.Resolve();
            if (resolved.IsInterface)
            {
                // the order is important here, the methods from lower level interfaces hide methods
                // with the same signature of their ancestors
                foreach (var method in resolved.Methods)
                {
                    if (!result.Any(m => HaveSameSignature(m, method)))
                        result.Add(method);
                }
                if (resolved.BaseType != null)
                    GetInterfaceMethods(resolved.BaseType, ref result);
            }
        }

        // Jeżeil klasa bazowa jest klasą abstrakcyjną, to nie ma znaczenia.
        // Klasa abstrakcyjna nie musi implementować wszystkich abstrakcyjnych metod odziedziczonych        
        private static void GetInheritedMethods(TypeDefinition type, ref List<MethodDefinition> result)
        {
            // Jeżeli dana klasa jest klasą abstrakcyjną, to do zwracanych metod musimy dodać wszystkie
            // niezaimplementowane abstrakcyjne metody wirtualne klasy bazowej. W innym przypadku, metody
            // te zostały z pewnością już zaimp
            if (type.BaseType != null)
            {
                var baseTypeMethods = new List<MethodDefinition>();

                TypeDefinition baseType = type.Module.MetadataResolver.Resolve(type.BaseType); //type.BaseType.Resolve();

                GetInheritedMethods(baseType, ref baseTypeMethods);

                foreach (var method in baseTypeMethods)
                {
                    if (!type.Methods.Any(m => HaveSameSignature(method, m)))
                    {
                        // all the virtual methods not implemented in current class
                        result.Add(method);
                    }
                }
            }

            foreach (var method in type.Methods.Where(m => m.IsVirtual && !m.IsFinal))
            {
                result.Add(method);
            }
        }

        private static bool HaveSameSignature(MethodDefinition method, MethodDefinition m)
        {
            if (method.Name != m.Name)
                return false;
            if (method.Parameters.Count != m.Parameters.Count)
                return false;

            var parameters1 = method.Parameters.Select(p => p.ParameterType).Except(m.Parameters.Select(p => p.ParameterType));
            var parameters = method.Parameters.Select(p => p.ParameterType.FullName).Except(m.Parameters.Select(p => p.ParameterType.FullName));

            int c1 = parameters1.Count(), c2 = parameters.Count();

            return parameters.Count() == 0;
        }

        public override void VisitMethodDefinition(Mono.Cecil.MethodDefinition method)
        {
            MethodDefinition baseTypeOrigin = null;
            if (method.IsVirtual && !method.IsNewSlot) //&& !method.IsAbstract)
            {
                var baseType = method.DeclaringType.BaseType;
                while (baseType != null)
                {
                    var baseTypeDefinition = baseType.Resolve();
                    baseTypeOrigin = baseTypeDefinition.Methods.SingleOrDefault(m => m.IsVirtual && !m.IsFinal && HaveSameSignature(m, method));
                    if (baseTypeOrigin != null)
                    {
                        AddOverride(method, baseTypeOrigin);
                        break;
                    }
                    else
                    {
                        baseType = baseTypeDefinition.BaseType;
                    }
                }
            }

            base.VisitMethodDefinition(method);
        }

        private void AddOverride(Mono.Cecil.MethodDefinition method, MethodDefinition baseTypeOrigin)
        {
            MethodReference methodReference = baseTypeOrigin;
            if (baseTypeOrigin.Module != method.Module)
                methodReference = method.Module.Import(baseTypeOrigin);

            if (methodOverrides.ContainsKey(method))
                methodOverrides[method].Add(methodReference);
            else
                methodOverrides.Add(method, new List<MethodReference>() { methodReference });
        }
    }
}
