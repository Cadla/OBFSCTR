using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System.IO;
using Obfuscator;
using Obfuscator.Steps;
using Obfuscator.Utils;
using Obfuscator.MetadataBuilder;
using Obfuscator.Common.Steps;

namespace Obfuscator.Merger
{
    public class MergeAssembliesStep : BaseStep
    {
        ReferenceResolver _resolver;
        AssemblyDefinition _mergedAssembly;
        IList<AssemblyDefinition> _assemblies;

        protected override void Process()
        {
             _assemblies = Context.GetAssemblies();
            _mergedAssembly = CloneAssemblyDefinition(_assemblies[0],/* "merged_" +*/ _assemblies[0].Name.Name);
            
            _resolver = new ReferenceResolver(_mergedAssembly.MainModule, t =>
            {
                return !_assemblies.Any(a => a.MainModule.GetAllTypes().Contains(t.Resolve()));
            });

            _resolver.Action = t =>
            {
                var resolved = t.Resolve();
                if (t.IsNested)
                {
                    TypeReference declaringType = _resolver.Action(resolved.DeclaringType);
                    return declaringType.Resolve().InjectNestedType(resolved, _resolver);
                }
                else
                    return _mergedAssembly.MainModule.InjectType(resolved, _resolver);
            };
        }

        protected override void ProcessAssembly(AssemblyDefinition assembly)
        {
            MetadataBuilderHelper.CopyCustomAttributes(assembly, _mergedAssembly, _resolver);
            MetadataBuilderHelper.CopyCustomAttributes(assembly.MainModule, _mergedAssembly.MainModule, _resolver);

            foreach (var resource in assembly.MainModule.Resources)
                _mergedAssembly.MainModule.InjectResource(resource);

            MetadataBuilderHelper.CopySecurityDeclarations(assembly, _mergedAssembly, _resolver);

            foreach (var type in assembly.MainModule.Types) // .ToList() ?
            {
                if (type.Name != "<Module>")
                {
                    _mergedAssembly.MainModule.InjectType(type, _resolver);
                }
            }
        }

        protected override void EndProcess()
        {            
            if (_assemblies[0].MainModule.EntryPoint != null)
            {
                var entryPointDeclaringType = _mergedAssembly.MainModule.GetType(_assemblies[0].MainModule.EntryPoint.DeclaringType.FullName);
                MethodDefinition entryPoint = null;
                if (Helper.TryGetMethod(entryPointDeclaringType.Methods, _assemblies[0].MainModule.EntryPoint, ref entryPoint))
                    _mergedAssembly.EntryPoint = entryPoint;
            }

            foreach (var assembly in _assemblies)
                Context.RemoveAssembly(assembly);

            Context.AddAssembly(_mergedAssembly);
        }


        //public AssemblyDefinition CopyAssembly(AssemblyDefinition sourceAssembly)
        //{
        //    if(sourceAssembly == null)
        //        throw new ArgumentNullException("sourceAssembly");

        //    AssemblyDefinition copy = CloneAssemblyDefinition(sourceAssembly, "copy_" + sourceAssembly.Name.Name);

        //    return Merge(new List<AssemblyDefinition> { sourceAssembly }, sourceAssembly, copy);
        //}

        private static AssemblyDefinition CloneAssemblyDefinition(AssemblyDefinition mainAssembly, String cloneAssemblyName)
        {
            AssemblyNameDefinition cloneName = new AssemblyNameDefinition(cloneAssemblyName, mainAssembly.Name.Version)
            {
                Attributes = mainAssembly.Name.Attributes,
                Culture = mainAssembly.Name.Culture,
                HashAlgorithm = mainAssembly.Name.HashAlgorithm,
                IsRetargetable = mainAssembly.Name.IsRetargetable,
                IsSideBySideCompatible = mainAssembly.Name.IsSideBySideCompatible,
                PublicKey = mainAssembly.Name.PublicKey,
            };

            AssemblyDefinition clone = AssemblyDefinition.CreateAssembly(cloneName, mainAssembly.MainModule.Name,
                new ModuleParameters()
                {
                    Kind = mainAssembly.MainModule.Kind,
                    Architecture = mainAssembly.MainModule.Architecture,
                    Runtime = mainAssembly.MainModule.Runtime,
                    AssemblyResolver = mainAssembly.MainModule.AssemblyResolver
                });

            return clone;
        }

        //private class MergedAssembly
        //{
        //    private IList<Node> graph = new List<Node>();

        //    public IList<TypeReference> Types
        //    {
        //        get;
        //        private set;
        //    }

        //    private enum NodeKind
        //    {
        //        // Core,
        //        Main,
        //        Satelite
        //    }

        //    public bool ValidateAssembly(out String message)
        //    {
        //        IList<AssemblyNameReference> conflictingAssemblies = new List<AssemblyNameReference>();
        //        StringBuilder result = new StringBuilder();

        //        foreach(var node in graph)
        //        {
        //            foreach(var reference in node.References)
        //            {
        //                if(!graph.Any(x => Helper.AreSame(x.Name, reference.Name)))
        //                    conflictingAssemblies.Add(node.Name);
        //            }
        //        }

        //        if(conflictingAssemblies.Count == 0)
        //        {
        //            message = "Assemblies validated";
        //            var main = graph.Single(x => x.Kind == NodeKind.Main);
        //            return true;
        //        }
        //        else
        //        {
        //            result.AppendLine("References of following assemblies will be lost due to merging:");
        //            foreach(var assembly in conflictingAssemblies)
        //            {
        //                result.AppendLine(assembly.Name);
        //            }
        //            message = result.ToString();
        //            return false;
        //        }
        //    }

        //    public MergedAssembly()
        //    {
        //        Types = new List<TypeReference>();
        //    }

        //    public void BuildMergedAssembly(IList<AssemblyDefinition> assemblies)
        //    {
        //        if(assemblies == null)
        //            throw new ArgumentNullException("assemblies");

        //        foreach(var assembly in assemblies)
        //        {
        //            Node node = CreateNode(assembly, NodeKind.Main, false);

        //            CopyTypes(assembly.MainModule.Types);
        //        }

        //        Merge();
        //    }

        //    public void BuildMergedAssembly(AssemblyDefinition assembly)
        //    {
        //        if(assembly == null)
        //            throw new ArgumentNullException("assembly");


        //        Node node = CreateNode(assembly, NodeKind.Main, true);


        //        Merge();
        //    }

        //    private void Merge()
        //    {
        //        var main = graph.Where(x => x.Kind == NodeKind.Main).ToList();
        //        HashSet<Node> internalGraph = new HashSet<Node>();
        //        HashSet<Node> references = new HashSet<Node>();

        //        foreach(var node in main)
        //        {
        //            internalGraph.Add(node);

        //            foreach(var reference in node.References)
        //                if(!main.Any(x => Helper.AreSame(x.Name, reference.Name)))
        //                    references.Add(reference);
        //        }

        //        var merged = new Node(graph[0].Name, NodeKind.Main, references, internalGraph);
        //        graph.Add(merged);

        //        foreach(var node in internalGraph)
        //            graph.Remove(node);
        //    }

        //    private Node CreateNode(AssemblyDefinition assembly, NodeKind kind, bool withDep)
        //    {
        //        var references = new HashSet<Node>();

        //        Node node;
        //        if((node = graph.FirstOrDefault(x => Helper.AreSame(x.Name, assembly.Name))) == null)
        //            node = new Node(assembly.Name, kind, references);
        //        else
        //        {
        //            if(kind == NodeKind.Main && node.Kind == NodeKind.Satelite)
        //            {
        //                node.Kind = NodeKind.Main;
        //                CopyTypes(assembly.MainModule.Types);
        //            }
        //            return node;
        //        }

        //        graph.Add(node);

        //        if(node.Kind == NodeKind.Main)
        //            CopyTypes(assembly.MainModule.Types);

        //        IAssemblyResolver resolver = assembly.MainModule.AssemblyResolver;

        //        try
        //        {
        //            foreach(var reference in assembly.MainModule.AssemblyReferences)
        //            {
        //                if(!Helper.IsCoreAssemblyName(reference.Name))
        //                {
        //                    var referenceDefinition = resolver.Resolve(reference);
        //                    var nodeKind = withDep ? NodeKind.Main : NodeKind.Satelite;
        //                    references.Add(CreateNode(referenceDefinition, nodeKind, withDep));
        //                }
        //            }
        //        }
        //        catch(FileNotFoundException)
        //        {
        //            throw;
        //        }
        //        return node;
        //    }

        //    private void CopyTypes(Collection<TypeDefinition> types)
        //    {
        //        foreach(var type in types)
        //        {
        //            if(type.Name != "<Module>")
        //            {
        //                Types.Add(type);
        //                if(type.HasNestedTypes)
        //                    CopyTypes(type.NestedTypes);
        //            }
        //        }
        //    }

        //    public bool HasType(TypeReference type)
        //    {
        //        if(Helper.IsCore(type))
        //            return false;
        //        if(Types.Any(x => x.GetElementType().FullName == type.FullName))
        //            return true;
        //        return false;
        //    }

        //    private class Node
        //    {
        //        HashSet<Node> internalGraph;

        //        public Node(AssemblyNameReference name, NodeKind kind, HashSet<Node> references)
        //        {
        //            internalGraph = new HashSet<Node>();
        //            if(name == null)
        //                throw new ArgumentNullException("name");
        //            if(references == null)
        //                throw new ArgumentNullException("dependencies");

        //            Name = name;
        //            Kind = kind;
        //            References = references;
        //        }

        //        public Node(AssemblyNameReference name, NodeKind kind, HashSet<Node> references, HashSet<Node> graph)
        //            : this(name, kind, references)
        //        {
        //            if(graph == null)
        //                throw new ArgumentNullException("graph");
        //        }

        //        public AssemblyNameReference Name
        //        {
        //            get;
        //            private set;
        //        }

        //        public NodeKind Kind
        //        {
        //            get;
        //            set;
        //        }

        //        public HashSet<Node> References
        //        {
        //            get;
        //            private set;
        //        }

        //        public override string ToString()
        //        {
        //            return Name.FullName;
        //        }
        //    }

        //}
    }
}

