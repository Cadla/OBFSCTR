using System;
using System.Collections.Generic;
using System.Text;

namespace Obfuscator.Renaming
{
    public interface IStringGenerator
    {
        string GetStartingString();
        string GetNext(string last);
    }
    
    public class StringGenerator : IStringGenerator
    {
        private string _alphabet;
        private StringBuilder builder = new StringBuilder();

        public StringGenerator(string alphabet)
        {
            _alphabet = alphabet;            
        }

        public string GetNext(string last)
        {
            builder.Clear();
            builder.Append(last);

            for (int i = last.Length - 1; i >= 0; i--)
            {
                int index = _alphabet.IndexOf(last[i]);
                if (index != _alphabet.Length - 1)
                {
                    builder[i] = _alphabet[index + 1];
                    return builder.ToString();
                }
                else
                {
                    builder[i] = _alphabet[0];
                }
            }
            builder.Insert(0, _alphabet[0]);
            return builder.ToString();
        }

        public string GetStartingString()
        {
            return "A";
        }
    }
}


// Although the CLR does allow an interface to define static methods, static fields, constants,
// and static constructors, a Common Language Infrastructure (CLI)–compliant interface must
// not have any of these static members because some programming languages aren’t able
// to define or access them. In fact, C# prevents an interface from defining any of these static
// members.