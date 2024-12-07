using Assembler.Parsing;

namespace Assembler.Tests.Parsing
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
            IEnumerable<string> inputLines = ["@R0"];
            var parser = new Parser(inputLines);
            Assert.IsTrue(parser.MoveNext());
        }

        [TestMethod]
        public void ResetEnumeration()
        {
            IEnumerable<string> inputLines = ["@R0"];
            var parser = new Parser(inputLines);
            Assert.IsTrue(parser.MoveNext());
            parser.Reset();
            Assert.IsTrue(parser.MoveNext());
            Assert.IsFalse(parser.MoveNext());
        }

        [TestMethod]
        [DataRow("@R0", CommandType.A_Command)]
        [DataRow("@10", CommandType.A_Command)]
        [DataRow("D=M", CommandType.C_Command)]
        [DataRow("D;JGT", CommandType.C_Command)]
        [DataRow("(TEST)", CommandType.L_Command)]
        [DataRow("(OUTPUT_FIRST)", CommandType.L_Command)]
        public void ReturnCorrectCommandType(string command, CommandType expectedCommandType)
        {
            IEnumerable<string> inputLines = [command];
            var parser = new Parser(inputLines);
            parser.MoveNext();
            Assert.AreEqual(expectedCommandType, parser.CommandType);
        }

        [TestMethod]
        [DataRow("@R0", "R0")]
        [DataRow("@10", "10")]
        [DataRow("(TEST)", "TEST")]
        [DataRow("(OUTPUT_FIRST)", "OUTPUT_FIRST")]
        [DataRow("D=M", null)]
        public void ReturnCorrectSymbol(string command, string expectedSymbol)
        {
            IEnumerable<string> inputLines = [command];
            var parser = new Parser(inputLines);
            parser.MoveNext();
            Assert.AreEqual(expectedSymbol, parser.Symbol);
        }

        [TestMethod]
        [DataRow("D=M", "D")]
        [DataRow("M=D", "M")]
        [DataRow("D;JGT", "null")]
        [DataRow("0;JMP", "null")]
        [DataRow("(TEST)", null)]
        [DataRow("@R0", null)]
        public void ReturnCorrectDest(string command, string expectedDest)
        {
            IEnumerable<string> inputLines = [command];
            var parser = new Parser(inputLines);
            parser.MoveNext();
            Assert.AreEqual(expectedDest, parser.Dest);
        }

        [TestMethod]
        [DataRow("D=M", "M")]
        [DataRow("M=D", "D")]
        [DataRow("D;JGT", "D")]
        [DataRow("0;JMP", "0")]
        [DataRow("(TEST)", null)]
        [DataRow("@R0", null)]
        public void ReturnCorrectComp(string command, string expectedComp)
        {
            IEnumerable<string> inputLines = [command];
            var parser = new Parser(inputLines);
            parser.MoveNext();
            Assert.AreEqual(expectedComp, parser.Comp);
        }

        [TestMethod]
        [DataRow("D=M", "null")]
        [DataRow("M=D", "null")]
        [DataRow("D;JGT", "JGT")]
        [DataRow("0;JMP", "JMP")]
        [DataRow("(TEST)", null)]
        [DataRow("@R0", null)]
        public void ReturnCorrectJump(string command, string expectedJump)
        {
            IEnumerable<string> inputLines = [command];
            var parser = new Parser(inputLines);
            parser.MoveNext();
            Assert.AreEqual(expectedJump, parser.Jump);
        }
    }
}
