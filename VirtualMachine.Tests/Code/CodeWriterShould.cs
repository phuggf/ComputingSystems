using VirtualMachine.Parsing;

namespace VirtualMachine.Tests.Code
{
    [TestClass]
    public class CodeWriterShould
    {
        [TestMethod]
        public void PushConstant()
        {
            var writer = new CodeWriter();

            var expected = new List<string>() { "@5", "D=A", "@SP", "M=M+1", "A=M-1", "M=D" };
            var observed = writer.WritePushPop(CommandType.C_PUSH, "constant", 5).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void PushLocal()
        {
            var writer = new CodeWriter();

            var expected = new List<string>() { "@LCL", "D=M", "@2", "A=D+A", "D=M", "@SP",
                                                "M=M+1", "A=M-1", "M=D" };
            var observed = writer.WritePushPop(CommandType.C_PUSH, "local", 2).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void PopLocal()
        {
            var writer = new CodeWriter();

            var expected = new List<string>() { "@LCL", "D=M", "@0", "D=D+A", "@R13", "M=D",
                                                "@SP", "AM=M-1", "D=M", "@R13", "A=M", "M=D" };
            var observed = writer.WritePushPop(CommandType.C_POP, "local", 0).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteArithmetic()
        {
            var command = "add";
            var writer = new CodeWriter();

            var expected = new List<string>() { "@SP", "A=M-1", "D=M", "A=A-1", "M=D+M", "D=A", "@SP", "M=D+1" };
            var observed = writer.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteEqualTo()
        {
            var command = "eq";
            var writer = new CodeWriter();

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

            var observed = writer.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteLessThan()
        {
            var command = "lt";
            var writer = new CodeWriter();

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

            var observed = writer.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }

        [TestMethod]
        public void WriteGreaterThan()
        {
            var command = "gt";
            var writer = new CodeWriter();

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

            var observed = writer.WriteArithmetic(command).ToList();

            for (int i = 0; i < observed.Count; i++)
            {
                Assert.AreEqual(expected[i], observed[i]);
            }
        }
    }
}
