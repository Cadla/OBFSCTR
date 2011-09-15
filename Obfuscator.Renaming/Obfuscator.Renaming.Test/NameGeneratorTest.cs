using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Obfuscator.Renaming.Test
{
    [TestClass]
    public class NameGeneratorTest
    {
        INameGenerator _nameGenerator;
        const string _alphabet = "ABC";

        [TestInitialize]
        public void SetUp()
        {
            _nameGenerator = new NameGenerator(_alphabet);
        }

        [TestMethod]
        public void TestNewGenerator()
        {
            var expected = "A";
            var actual = _nameGenerator.GetNext("");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWholeAlphabet()
        {
            string[] expected = { "A", "B", "C", "AA", "AB", "AC", "BA", "BB", "BC", "CA", "CB", "CC" };
            for (int i = 1; i < expected.Length; ++i)
            {
                var actual = _nameGenerator.GetNext(expected[i - 1]);
                Assert.AreEqual(actual, expected[i]);
            }                        
        }
    }
}
