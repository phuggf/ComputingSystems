namespace Assembler.Tests
{
    [TestClass]
    public class AssemblerShould
    {
        [TestMethod]
        public void ConvertAsmToBinary_Integration()
        {
            var input = File.ReadLines("Pong.asm");

            var expectedOutput = File.ReadLines("Pong.hack").ToArray();

            var assembler = new Assembler();
            var observedOutput = assembler.Assemble(input).ToArray();

            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual( expectedOutput[i], observedOutput[i] );
            }

            Assert.AreEqual(expectedOutput.Length, observedOutput.Length);
        }

        [TestMethod]
        public void ConvertToBinaryWithPredefinedSymbols()
        {
            string[] input =
            [
                   "@R0",
            ];

            string[] expectedOutput =
            [
                "0000000000000000",
            ];

            var assembler = new Assembler();
            var observedOutput = assembler.Assemble(input).ToArray();

            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], observedOutput[i]);
            }

            Assert.AreEqual(expectedOutput.Length, observedOutput.Length);
        }

        [TestMethod]
        public void ConvertToBinaryWithLabel()
        {
            string[] input =
            [
                   "@Label",
                   "(Label)",
                   "D=M",
            ];

            string[] expectedOutput =
            [
                "0000000000000001",
                "1111110000010000"
            ];

            var assembler = new Assembler();
            var observedOutput = assembler.Assemble(input).ToArray();


            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], observedOutput[i]);
            }

            Assert.AreEqual(expectedOutput.Length, observedOutput.Length);
        }

        [TestMethod]
        public void ConvertToBinaryWithVariablesHavingNextAvailableAddress()
        {
            string[] input =
            [
                   "@i",
                   "D=M",
            ];

            string[] expectedOutput =
            [
                "0000000000010000",
                "1111110000010000"
            ];

            var assembler = new Assembler();
            var observedOutput = assembler.Assemble(input).ToArray();

            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], observedOutput[i]);
            }

            Assert.AreEqual(expectedOutput.Length, observedOutput.Length);
        }

        [TestMethod]
        public void ConvertToBinaryWithLabels()
        {
            string[] input =
            [
                   "@R0",
                   "D=M",
                   "@R1",
                   "D=D-M",
                   "@OUTPUT_FIRST",
                   "D;JGT",
                   "@R1",
                   "D=M",
                   "@OUTPUT_D",
                   "0;JMP",
                "(OUTPUT_FIRST)",
                   "@R0",
                   "D=M",
                "(OUTPUT_D)",
                   "@R2",
                   "M=D",
                "(INFINITE_LOOP)",
                   "@INFINITE_LOOP",
                   "0;JMP",
            ];

            string[] expectedOutput =
            [
                "0000000000000000",
                "1111110000010000",
                "0000000000000001",
                "1111010011010000",
                "0000000000001010",
                "1110001100000001",
                "0000000000000001",
                "1111110000010000",
                "0000000000001100",
                "1110101010000111",
                "0000000000000000",
                "1111110000010000",
                "0000000000000010",
                "1110001100001000",
                "0000000000001110",
                "1110101010000111",
            ];

            var assembler = new Assembler();
            var observedOutput = assembler.Assemble(input).ToArray();

            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], observedOutput[i]);
            }

            Assert.AreEqual(expectedOutput.Length, observedOutput.Length);
        }
    }
}