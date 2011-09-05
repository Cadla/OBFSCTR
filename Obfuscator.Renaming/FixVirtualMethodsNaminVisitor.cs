using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Utils;
using Mono.Cecil;

namespace Obfuscator.Renaming
{
    public class FixVirtualMethodsNaminVisitor : NullAssemblyVisitor
    {
        private ScopeSth _scope;
        private IDictionary<string, string> _renameMap;      
  
        public override void VisitMethodDefinition(Mono.Cecil.MethodDefinition method)
        {
            while(HasAnOriginMethod(method))
                Rename(method);

            base.VisitMethodDefinition(method);
        }

        private bool HasAnOriginMethod(Mono.Cecil.MethodDefinition method)
        {
            if (method.IsVirtual && !method.IsNewSlot) //&& !method.IsAbstract)
            {
                var baseType = method.DeclaringType.BaseType;
                while (baseType != null)
                {
                    var baseTypeDefinition = baseType.Resolve();
                    if (baseTypeDefinition.Methods.SingleOrDefault(m => m.IsVirtual /*&& !m.IsFinal*/ && HaveSameSignature(m, method)) != null)
                    {
                        return true;
                    }
                    baseType = baseTypeDefinition.BaseType;
                }
            }
            return false;
        }

        public FixVirtualMethodsNaminVisitor(ScopeSth scope, IDictionary<string, string> renameMap)
        {
            _scope = scope;
            _renameMap = renameMap;
        }
            
        private void Rename(MethodDefinition member)
        {
            string oldName = member.FullName;
            string newName = _scope.GetName(member);

   
            member.Name = newName;
            
            // validates uniqueness
            _renameMap[oldName] = member.FullName;
        }

        private static bool HaveSameSignature(MethodDefinition method, MethodDefinition m)
        {
            if (method.Name != m.Name)
                return false;
            if (method.Parameters.Count != m.Parameters.Count)
                return false;

            var parameters = method.Parameters.Select(p => p.ParameterType).Except(m.Parameters.Select(p => p.ParameterType));
            return parameters.Count() == 0;
        }
    }
}
