using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Renaming.Reflection
{
    internal static class ReflectionMethodsHelper
    {
        // System.Type.GetType(string)
        // System.Type.GetType(string, bool)
        // System.Type.GetType(string, bool, bool)
        public static bool IsGetType(MethodReference method)
        {
            var declaringTypeName = method.DeclaringType.FullName;
            if (declaringTypeName != "System.Type")
                return false;

            if (method.Name != "GetType")
                return false;

            if (method.HasParameters && method.Parameters[0].ParameterType.FullName == "System.String")
                return true;

            return false;
        }

        // System.Type::GetEvent(string)
        // System.Type::GetEvent(string, BindingFlags)
        public static bool IsGetEvent(MethodReference method)
        {
            if (!IsSupportedSystemType(method))
                return false;

            if (method.Name == "GetEvent")
                return true;

            return false;
        }

        // System.Type::GetField(string)
        // System.Type::GetField(string, BindingFlags)
        public static bool IsGetField(MethodReference method)
        {
            if (!IsSupportedSystemType(method))
                return false;

            if (method.Name == "GetField")
                return true;

            return false;
        }

        // System.Type::GetNestedType(string)
        // System.Type::GetNestedType(string, BindingFlags)
        public static bool IsGetNestedType(MethodReference method)
        {
            if (!IsSupportedSystemType(method))
                return false;

            if (method.Name == "GetNestedType")
                return true;

            return false;
        }

        // System.Type::GetMethod(string, Type[])
        public static bool IsGetMethod(MethodReference method)
        {
            if (!IsSupportedSystemType(method))
                return false;

            if (method.Parameters.Count != 2)
                return false;

            if (method.Name != "GetMethod")
                return false;

            if (method.Parameters[0].ParameterType.FullName == "System.String" && method.Parameters[1].ParameterType.FullName == "System.Type[]")
                return true;

            return false;
        }

        // System.Type::GetProperty(string, Type[])
        public static bool IsGetProperty(MethodReference method)
        {
            if (!IsSupportedSystemType(method))
                return false;

            if (method.Parameters.Count != 2)
                return false;

            if (method.Name != "GetProperty")
                return false;

            if (method.Parameters[0].ParameterType.FullName == "System.String" && method.Parameters[1].ParameterType.FullName == "System.Type[]")
                return true;

            return false;
        }

        // System.Resources.ResourceManager..ctor
        public static bool IsCreateResourceManager(MethodReference method)
        {
            if (method.DeclaringType.FullName != "System.Resources.ResourceManager")
                return false;

            if (method.Name != ".ctor")
                return false;

            if (method.HasParameters && method.Parameters[0].ParameterType.FullName == "System.String")
                return true;

            return false;
        }

        // System.Reflaction.Assembly::GetType(string)
        // System.Reflaction.Assembly::GetType(string, bool)
        // System.Reflaction.Assembly::GetType(string, bool, bool)
        public static bool IsGetTypeFromAssembly(MethodReference method)
        {
            if (method.DeclaringType.FullName != "System.Reflection.Assembly")
                return false;

            if (method.Name != "GetType")
                return false;

            if (method.HasParameters && method.Parameters[0].ParameterType.FullName == "System.String")
                return true;

            return true;
        }

        private static bool IsSupportedSystemType(MethodReference method)
        {
            if (method.DeclaringType.FullName != "System.Type" || method.HasThis == false)
                return false;

            if (method.Name == "GetInterface" || method.Name == "GetTypeFromProgID")
                return false;

            return true;
        }
    }
}
