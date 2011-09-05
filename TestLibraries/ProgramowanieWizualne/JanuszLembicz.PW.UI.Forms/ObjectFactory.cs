#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace JanuszLembicz.Utils
{
    public class ObjectFactory : AbstractSingleton<ObjectFactory>
    {
        private readonly Dictionary<Type, object> _builderTypes = new Dictionary<Type, object>();
        private Assembly _assembly;
        private String _assemblyName;

        public String AssemblyName
        {
            get
            {
                return _assemblyName;
            }

            set
            {
                if(_assemblyName != value)
                {
                    _assemblyName = value;
                    _assembly = Assembly.LoadFrom(_assemblyName);
                    _builderTypes.Clear();
                }
            }
        }

        public T CreateInstance<T>()
        {
            Type interfaceType = typeof(T);
            if(!_builderTypes.ContainsKey(interfaceType))
            {
                foreach(Type type in _assembly.GetTypes())
                {
                    if(type.IsClass && interfaceType.IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        _builderTypes[interfaceType] = Activator.CreateInstance(type);
                        return (T)_builderTypes[interfaceType];
                    }
                }
            }
            return (T)_builderTypes[interfaceType];
        }
    }
}