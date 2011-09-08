using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.IO;
using Mono.Cecil.Cil;

namespace Obfuscator.Renaming
{
    public class Renamer
    {        
         ScopeNameGenerator _scopeNameGenerator;

         public IDictionary<IMemberDefinition, string> RenameMap
         {
             get;
             private set;
         }

         public Dictionary<Resource, string> ResourcesNames
         {
             get;
             private set;
         }

         public HashSet<Instruction> LdstrInstructions
         {
             get;
             set;
         }

         public Dictionary<string, string> ReplacedStrings
         {
             get;
             private set;
         }

         public Renamer(INameGenerator nameGenerator)
         {
             _scopeNameGenerator = new ScopeNameGenerator(nameGenerator);
             RenameMap = new Dictionary<IMemberDefinition, string>();
             ResourcesNames = new Dictionary<Resource, string>();
             ReplacedStrings = new Dictionary<string, string>();
             LdstrInstructions = new HashSet<Instruction>();
         }

         //TODO Same fully-qualified type names in two assemblies http://msdn.microsoft.com/en-us/library/ms173212.aspx
         public string MapNewName(IMemberDefinition member)
         {
             string newName = _scopeNameGenerator.GetName(member);
             RenameMap[member] = newName;
             return newName;
         }

         public string MapNewName(Resource resource)
         {
             string name = Path.GetFileNameWithoutExtension(resource.Name);
             string newName;
             if ((newName = GetMappedName(name)) != null)
             {
                 string suffix = resource.Name.Substring(name.Length);
                 newName = newName + suffix;
                 ResourcesNames[resource] = newName;
                 return newName;
             }
             return name;
         }
         
         public void MapInstruction(Instruction @string){
             LdstrInstructions.Add(@string);
         }

         public string GetMappedName(IMemberDefinition member)
         {
             if (RenameMap.ContainsKey(member))
             {
                 return RenameMap[member];
             }
             return null;
         }

         public string GetMappedName(Resource resource)
         {
             if (ResourcesNames.ContainsKey(resource))
             {
                 return ResourcesNames[resource];
             }
             return null;
         }
        
         private string GetMappedName(string memberName)
         {
             IMemberDefinition member;
             if ((member = RenameMap.Keys.SingleOrDefault(m => m.FullName == memberName)) != null)
             {
                 return RenameMap[member];
             }
             return null;
         }
         
         public string Rename(IMemberDefinition member)
         {
             string mappedName;
             if ((mappedName = GetMappedName(member)) != null)
             {
                 if (member is TypeDefinition)
                 {
                     ((TypeDefinition)member).Namespace = "";
                 }
                 return member.Name = mappedName;
             }
             else
             {
                 return member.Name = MapNewName(member);
             }
         }

         public string Rename(Resource resource)
         {
             string mappedName;
             if ((mappedName = GetMappedName(resource)) != null)
             {
                 return resource.Name = mappedName;
             }
             else
             {
                 return resource.Name = MapNewName(resource);
             }

         }

         public void RenameAll()
         {
             ReplaceStrings();
             RenameResources();

             foreach (var member in RenameMap)
                 Rename(member.Key);             
         }

         private void RenameResources()
         {
             foreach (var resource in ResourcesNames)
                Rename(resource.Key);             
         }

         private void ReplaceStrings()
         {             
             foreach (var str in LdstrInstructions)
             {
                 var operand = str.Operand as string;
                 if (RenameMap.Any(k => k.Key.FullName == operand))
                 {                     
                     var x = RenameMap.Single(k => k.Key.FullName == operand);
                     ReplacedStrings[operand] = x.Value;
                     str.Operand = x.Value;
                 }
             }             
         }
    }
}
