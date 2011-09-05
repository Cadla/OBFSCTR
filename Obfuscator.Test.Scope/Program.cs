using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace Obfuscator.Test.Scope
{   
    public class Program
    {        
        static void Main(string[] args)
        {
            NameConflictBase b = new NameConflictBase();
            NameConfictChild c = new NameConfictChild();

            System.Console.WriteLine("NameConflictBase A() {0}", b.A());
            System.Console.WriteLine("NameConflictBase AMethod() {0}", b.AMethod());

            System.Console.WriteLine("NameConflictChild B() {0}", c.B());
            System.Console.WriteLine("NameConflictChild BMethod() {0}", c.BMethod());

            System.Console.WriteLine("NameConflictChild A() {0}", c.A());
            System.Console.WriteLine("NameConflictChild AMethod() {0}", c.AMethod());

            System.Console.WriteLine("NameConflictChild AMethod() {0}", c.AMethod());

            System.Console.WriteLine("Base C {0}, Child C {1}, Child A {2}", b.C, c.C, c.A);
            System.Console.ReadKey();
        }
    }
}
