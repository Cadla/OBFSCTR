﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscator.Configuration.COM;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Obfuscator.Common.Configuration
{
    public sealed class DefaultConfiguration
    {

        //private List<Assembly> _assemblies = new List<Assembly>();
        //private List<Assembly> _referencingAssemblies = new List<Assembly>();

        //public List<Assembly> Assemblies
        //{
        //    get
        //    {
        //        return _assemblies;
        //    }
        //}

        //public List<Assembly> ReferencingAssemblies
        //{
        //    get
        //    {
        //        return _referencingAssemblies;
        //    }
        //}


        //private HashSet<Member> EntryPoints
        //{
        //    get;
        //    set;
        //}

        //private Dictionary<COM.Type, int> AccessedByN
        //{
        //    get;
        //    set;
        //}

        //private Dictionary<Method, int> InvokedByN
        //{
        //    get;
        //    set;
        //}

        //private List<Method> PreserveStrings
        //{
        //    get;
        //    set;
        //}

        //private List<Assembly> KeepNamespaces
        //{
        //    get;
        //    set;
        //}

        //protected override bool IsEntryPoint(Member member)
        //{
        //    if (EntryPoints == null)
        //        SetEntryPoints();

        //    if (EntryPoints.Contains(member))
        //        return true;
        //    else if (EntryPoints.Contains(member.DeclaringType))
        //        return true;

        //    return false;
        //}

        //protected override bool ShouldKeepNamespacess(Assembly assembly)
        //{
        //    return false;
        //}

        ////protected override bool InvokedByName(Method method, out int nameIndex, out int typeInstanceIndex)
        ////{
        ////    if (InvokedByN == null)
        ////        SetInvokeByName();
        ////    typeInstanceIndex = -1;

        ////    if (InvokedByN.TryGetValue(method, out nameIndex))
        ////        return true;
        ////    return false;
        ////}

        ////protected override bool AccessedByName(COM.Type type, out int nameIndex)
        ////{
        ////    throw new NotImplementedException();
        ////}
        

        //private void SetEntryPoints()
        //{
        //    var skipList = new HashSet<Member>();
        //    //foreach (var assembly in Assemblies)
        //    //{
        //    //    var types = Types(assembly);
        //    //    foreach (var type in types)
        //    //    {
        //    //        if (type.Name == "Settings")
        //    //            skipList.Add(type);                        
        //    //    }
        //    //}
        //    EntryPoints = skipList;
        //}
     
   
    }
}
