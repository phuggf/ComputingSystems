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

namespace Compiler.Tests
{
    [TestClass]
    public class JackAnalyserShould
    {
        [TestMethod]
        public void ConvertJackFileToXml()
        {
            var inputLines = File.ReadLines("Main.jack").ToArray();
            var expectedOutput = File.ReadLines("Main.xml").ToArray();

            var jackAnalyser = new JackAnalyser();
            var observedOutput = jackAnalyser.Compile(inputLines).ToArray();

            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], observedOutput[i]);
            }

            Assert.AreEqual(expectedOutput.Length, observedOutput.Length);
        }
    }
}
