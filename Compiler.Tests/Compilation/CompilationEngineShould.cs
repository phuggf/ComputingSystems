using Compiler.Compilation;
using Compiler.Tokenising;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Compiler.Tests.Compilation
{
    [TestClass]
    public class CompilationEngineShould
    {
        [TestMethod]
        public void IntegrationTest()
        {
            var inputFilter = new InputFilter();
            var inputLines = inputFilter.Filter(File.ReadLines("Main.jack"));

            var expectedOutput = File.ReadLines("Main.xml").ToArray();
            var tokeniser = new Tokeniser(inputLines);

            var compilationEngine = new CompilationEngine(tokeniser);

            var tokenElements = new List<XElement>();
        }
    }
}
