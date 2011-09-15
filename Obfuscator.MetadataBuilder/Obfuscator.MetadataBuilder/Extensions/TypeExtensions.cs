using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Obfuscator.Utils;


// NOTE umozliwienie kopiowania osobno: pol, metod typow moze prowadzic do naruszania ograniczen (internal) i ciagnie za soba cala fale roznych konsekwencji
// np. skopiowanie metody korzystajacej z prywatnych pol w klasie prowadzi do bledow itp.
namespace Obfuscator.MetadataBuilder.Extensions
{
    public static class TypeExtensions
    {
        public static FieldDefinition InjectField(this TypeDefinition targetType, FieldDefinition sourceField)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");
            if (sourceField == null)
                throw new ArgumentNullException("sourceField");          

            FieldDefinition newField;
            if (Helper.TryGetField(targetType.Fields, sourceField, ref newField))
                return newField;

            TypeReference fieldType; // GetMemberReference(sourceField.FieldType, targetType);

            newField = new FieldDefinition(sourceField.Name, sourceField.Attributes, fieldType)
            {
                InitialValue = sourceField.InitialValue,
                DeclaringType = targetType,
            };

            targetType.Fields.Add(newField);

            CopyCustomAttributes(sourceField, newField);

            if (newField.HasDefault)
                newField.Constant = sourceField.Constant;

            return newField;

        }
        
    }
}
