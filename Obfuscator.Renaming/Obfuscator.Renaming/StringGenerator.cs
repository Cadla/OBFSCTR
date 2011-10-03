using System.Text;

namespace Obfuscator.Renaming
{
    // TODO: Creat not-printable strings generator
        //public static string GetNotPrintableASCIIChars()
        //{
        //    StringBuilder builder = new StringBuilder();
        //    for (int i = 1; i < 32; i++)
        //        builder.Append((char)i);
        //    return builder.ToString();
        //}
 
    public class StringGenerator 
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
            return "_"+builder.ToString();
        }
                
        public string GetStartingString()
        {
            return _alphabet[0].ToString();
        }
    }

}


// Although the CLR does allow an interface to define static methods, static fields, constants,
// and static constructors, a Common Language Infrastructure (CLI)–compliant interface must
// not have any of these static members because some programming languages aren’t able
// to define or access them. In fact, C# prevents an interface from defining any of these static
// members.