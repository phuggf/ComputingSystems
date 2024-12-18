using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using VirtualMachine.Parsing;

namespace VirtualMachine.Tests.Code
{
    [TestClass]
    public class CodeWriterShould
    {
        private CodeWriter _codeWriter;

        [TestInitialize]
        public void Initialize()
        {
            _codeWriter = new CodeWriter();
        }

        [TestMethod]
        public void PushConstant()
        {
            var expected = new List<string>() { "@5", "D=A", "@SP", "M=M+1", "A=M-1", "M=D" };
            var observed = _codeWriter.WritePush("constant", 5).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void PushLocal()
        {
            var expected = new List<string>() { "@LCL", "D=M", "@2", "A=D+A", "D=M", "@SP",
                                                "M=M+1", "A=M-1", "M=D" };
            var observed = _codeWriter.WritePush("local", 2).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void PopLocal()
        {
            var expected = new List<string>() { "@LCL", "D=M", "@0", "D=D+A", "@R13", "M=D",
                                                "@SP", "AM=M-1", "D=M", "@R13", "A=M", "M=D" };
            var observed = _codeWriter.WritePop("local", 0).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteArithmetic()
        {
            var command = "add";

            var expected = new List<string>() { "@SP", "A=M-1", "D=M", "A=A-1", "M=D+M", "D=A", "@SP", "M=D+1" };
            var observed = _codeWriter.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteEqualTo()
        {
            var command = "eq";

            var expected = new List<string>()
            {
                "@SP",
                "AM=M-1",
                "D=M",
                "A=A-1",
                "D=D-M",
                "@T0",
                "D;JEQ",
                "@SP",
                "AM=M-1",
                "M=0",
                "@E0",
                "0;JMP",
                "(T0)",
                "@SP",
                "AM=M-1",
                "M=-1",
                "(E0)",
                "@SP",
                "M=M+1",
            };

            var observed = _codeWriter.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteLessThan()
        {
            var command = "lt";

            var expected = new List<string>()
            {
                "@SP",
                "AM=M-1",
                "D=M",
                "A=A-1",
                "D=D-M",
                "@T0",
                "D;JGT",
                "@SP",
                "AM=M-1",
                "M=0",
                "@E0",
                "0;JMP",
                "(T0)",
                "@SP",
                "AM=M-1",
                "M=-1",
                "(E0)",
                "@SP",
                "M=M+1",
            };

            var observed = _codeWriter.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteGreaterThan()
        {
            var command = "gt";

            var expected = new List<string>()
            {
                "@SP",
                "AM=M-1",
                "D=M",
                "A=A-1",
                "D=D-M",
                "@T0",
                "D;JLT",
                "@SP",
                "AM=M-1",
                "M=0",
                "@E0",
                "0;JMP",
                "(T0)",
                "@SP",
                "AM=M-1",
                "M=-1",
                "(E0)",
                "@SP",
                "M=M+1",
            };

            var observed = _codeWriter.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteInitialisation()
        {
            var expected = new List<string>()
            {
                "@256",
                "D=A",
                "@SP",
                "M=D",
            };

            var observed = _codeWriter.WriteInit().ToList();

            // use expected.Count since we are just interested in the first four lines
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        [DataRow("label", null, "(label)")]
        public void WriteLabel( string label, string currentFunction, string expected)
        {
            var observed = _codeWriter.WriteLabel(label).Single();

            Assert.AreEqual(expected, observed);
        }

        [TestMethod]
        [DataRow("label", null, "@label")]
        public void WriteGoto(string label, string currentFunction, string expected)
        {
            var observed = _codeWriter.WriteGoto(label).ToList();

            Assert.AreEqual(expected, observed[0]);
            Assert.AreEqual("0;JMP", observed[1]);
        }

        [TestMethod]
        public void WriteIf()
        {
            var expected = new List<string>()
            {
                "@SP",
                "AM=M-1",
                "D=M",
                "@label", //$"@{(_currentFunction == null ? label : $"{_currentFunction}${label}")}",
                "D;JLT",
            };

            var observed = _codeWriter.WriteIf("label").ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteCall()
        {
            var observed = _codeWriter.WriteCall("name", 0).ToList();

            Assert.AreEqual("@return-address0", observed[0]);
            Assert.IsTrue(observed.Contains("@name"));
        }

        [TestMethod]
        public void WriteReturn()
        {
            var observed = _codeWriter.WriteReturn();

            Assert.AreEqual("@LCL", observed.First());
            Assert.AreEqual("0;JMP", observed.Last());
        }

        [TestMethod]
        public void WriteFunction()
        {
            var codeWriter = new CodeWriter(); // new instance as will change state
            var observed = codeWriter.WriteFunction("name", 0).Single();

            Assert.AreEqual("(name)", observed);
        }

        [TestMethod]
        public void ContainCurrentFunctionInLabel()
        {
            var codeWriter = new CodeWriter(); // new instance as will change state
            var functionLines = codeWriter.WriteFunction("functionName", 0).Single();
            var labelLine = codeWriter.WriteLabel("label").Single();

            Assert.AreEqual("(functionName$label)", labelLine);
        }
    }
}
