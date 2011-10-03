using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Configuration.COM
{
    public static class Extensions
    {
        public static IQueryable<COM.Type> Types(this Assembly assembly)
        {
            var types = assembly.AssemblyDefinition.MainModule.GetTypes();
            return types.Where(t => t.Name != "<Module>").Select(t => new COM.Type(t)).AsQueryable();
        }

        public static IQueryable<Method> Methods(this COM.Type type)
        {
            return type.TypeDefinition.Methods.Select(m => new Method(m)).AsQueryable();
        }

        public static IQueryable<Property> Properties(this COM.Type type)
        {
            return type.TypeDefinition.Properties.Select(p => new Property(p)).AsQueryable();
        }

        public static IQueryable<Field> Fields(this COM.Type type)
        {
            return type.TypeDefinition.Fields.Select(f => new Field(f)).AsQueryable();
        }

        public static IQueryable<Event> Events(this COM.Type type)
        {
            return type.TypeDefinition.Events.Select(e => new Event(e)).AsQueryable();
        }

        public static IQueryable<COM.Type> NestedTypes(this COM.Type type)
        {
            return type.TypeDefinition.NestedTypes.Select(e => new COM.Type(e)).AsQueryable();
        }

        public static IQueryable<Member> Members(this COM.Type type)
        {
            return type.Methods().Union<Member>(type.Properties()).Union(type.Fields()).Union(type.Events()).Union(type.NestedTypes());
        }
    }
}
