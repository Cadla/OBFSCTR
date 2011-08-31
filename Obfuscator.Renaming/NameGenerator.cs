using System;
using System.Collections.Generic;
using System.Text;

namespace Obfuscator.Renaming
{
    public class NameGenerator
    {
        private string _alphabet;
        private const int _maxLength = 20;

        // TODO change StringBuilder to string
        IDictionary<string, StringBuilder> names = new Dictionary<string, StringBuilder>();

        public NameGenerator(string alphabet)
        {
            _alphabet = alphabet;
        }

        // LOOKUP Path.GetRandomFileName BaseBMethod

        public string GetName(string scope)
        {
            if (!names.ContainsKey(scope))
            {
                names[scope] = new StringBuilder();
            }
            GetNext(names[scope]);
            return names[scope].ToString();
        }

        private void GetNext(StringBuilder last)
        {
            for (int i = last.Length - 1; i >= 0; i--)
            {
                int index = _alphabet.IndexOf(last[i]);
                if (index != _alphabet.Length - 1)
                {
                    last[i] = _alphabet[index + 1];
                    return;
                }
                else
                {
                    last[i] = _alphabet[0];
                }
            }
            last.Insert(0, _alphabet[0]);

            if (last.Length > _maxLength)
                // TODO change to specific exception type
                throw new Exception();

        }
    }
}


// Although the CLR does allow an interface to define static methods, static fields, constants,
// and static constructors, a Common Language Infrastructure (CLI)–compliant interface must
// not have any of these static members because some programming languages aren’t able
// to define or access them. In fact, C# prevents an interface from defining any of these static
// members.