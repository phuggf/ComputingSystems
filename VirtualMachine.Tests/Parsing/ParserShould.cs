using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualMachine.Parsing;

namespace VirtualMachine.Tests.Parsing
{
    [TestClass]
    public class ParserShould
    {
        [TestMethod]
        [DataRow([])]
        public void ReturnFalseWhenNoMoreCommands(string[] inputLines)
        {
            var parser = new Parser(inputLines);
            Assert.IsFalse(parser.MoveNext());
        }

        [TestMethod]
        public void ReturnTrueWhenMoreCommands()
        {
            IEnumerable<string> inputLines = ["push constant 10"];
            var parser = new Parser(inputLines);
            Assert.IsTrue(parser.MoveNext());
        }

        [TestMethod]
        [DataRow("push constant 7", CommandType.C_PUSH)]
        [DataRow("pop local 0", CommandType.C_POP)]
        [DataRow("add", CommandType.C_ARITHMETIC)]
        public void ReturnCorrectCommandType(string command, CommandType expectedCommandType)
        {
            IEnumerable<string> inputLines = [command];
            var parser = new Parser(inputLines);
            parser.MoveNext();
            Assert.AreEqual(expectedCommandType, parser.CommandType);
        }

        [TestMethod]
        [DataRow("push constant 7", "constant")]
        [DataRow("push local 0", "local")]
        [DataRow("add", "add")] // specified in book
        public void ReturnCorrectArg1(string command, string expectedArg1)
        {
            IEnumerable<string> inputLines = [command];
            var parser = new Parser(inputLines);
            parser.MoveNext();
            Assert.AreEqual(expectedArg1, parser.Arg1);
        }

        [TestMethod]
        [DataRow("push constant 7", "7")]
        [DataRow("function mult 2", "2")]
        [DataRow("push local 0", "0")]
        [DataRow("add", null)]
        public void ReturnCorrectArg2(string command, string expectedArg1)
        {
            IEnumerable<string> inputLines = [command];
            var parser = new Parser(inputLines);
            parser.MoveNext();
            Assert.AreEqual(expectedArg1, parser.Arg2);
        }
    }
}
