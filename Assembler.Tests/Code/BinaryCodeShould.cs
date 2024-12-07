using Assembler.Code;

namespace Assembler.Tests.Code
{
    [TestClass]
    public class BinaryCodeShould
    {
        [TestMethod]
        [DataRow("null", 0b000)]
        [DataRow("M",    0b001)]
        [DataRow("D",    0b010)]
        [DataRow("MD",   0b011)]
        [DataRow("A",    0b100)]
        [DataRow("AM",   0b101)]
        [DataRow("AD",   0b110)]
        [DataRow("AMD",  0b111)]
        public void ReturnDestCodes( string input, int expectedOutput)
        {
            var binaryCodes = new BinaryCodes();
            Assert.AreEqual(expectedOutput, binaryCodes.Dest(input));
        }

        [TestMethod]
        [DataRow("null", 0b000)]
        [DataRow("JGT", 0b001)]
        [DataRow("JEQ", 0b010)]
        [DataRow("JGE", 0b011)]
        [DataRow("JLT", 0b100)]
        [DataRow("JNE", 0b101)]
        [DataRow("JLE", 0b110)]
        [DataRow("JMP", 0b111)]
        public void ReturnJumpCodes(string input, int expectedOutput)
        {
            var binaryCodes = new BinaryCodes();
            Assert.AreEqual(expectedOutput, binaryCodes.Jump(input));
        }

        [TestMethod]
        // comp when a=0
        [DataRow("0",  0b0_101010)]
        [DataRow("1",  0b0_111111)]
        [DataRow("-1", 0b0_111010)]
        [DataRow("D",  0b0_001100)]
        [DataRow("A",  0b0_110000)]
        [DataRow("!D", 0b0_001101)]
        [DataRow("!A", 0b0_110001)]
        [DataRow("-D", 0b0_001111)]
        [DataRow("-A", 0b0_110011)]
        [DataRow("D+1",0b0_011111)]
        [DataRow("A+1",0b0_110111)]
        [DataRow("D-1",0b0_001110)]
        [DataRow("A-1",0b0_110010)]
        [DataRow("D+A",0b0_000010)]
        [DataRow("D-A",0b0_010011)]
        [DataRow("A-D",0b0_000111)]
        [DataRow("D&A",0b0_000000)]
        [DataRow("D|A",0b0_010101)]
        // comp when a=1
        [DataRow("M",  0b1_110000)]
        [DataRow("!M", 0b1_110001)]
        [DataRow("-M", 0b1_110011)]
        [DataRow("M+1",0b1_110111)]
        [DataRow("M-1",0b1_110010)]
        [DataRow("D+M",0b1_000010)]
        [DataRow("D-M",0b1_010011)]
        [DataRow("M-D",0b1_000111)]
        [DataRow("D&M",0b1_000000)]
        [DataRow("D|M",0b1_010101)]
        public void ReturnCompCodes(string input, int expectedOutput)
        {
            var binaryCodes = new BinaryCodes();
            Assert.AreEqual(expectedOutput, binaryCodes.Comp(input));
        }

        [DataRow("SP",  0x0000)]
        [DataRow("LCL", 0x0001)]
        [DataRow("ARG", 0x0002)]
        [DataRow("THIS",0x0003)]
        [DataRow("THAT",0x0004)]
        [DataRow("R0",  0x0000)]
        [DataRow("R1",  0x0001)]
        [DataRow("R2",  0x0002)]
        [DataRow("R3",  0x0003)]
        [DataRow("R4",  0x0004)]
        [DataRow("R5",  0x0005)]
        [DataRow("R6",  0x0006)]
        [DataRow("R7",  0x0007)]
        [DataRow("R8",  0x0008)]
        [DataRow("R9",  0x0009)]
        [DataRow("R10", 0x000A)]
        [DataRow("R11", 0x000B)]
        [DataRow("R12", 0x000C)]
        [DataRow("R13", 0x000D)]
        [DataRow("R14", 0x000E)]
        [DataRow("R15", 0x000F)]
        [DataRow("SCREEN", 0x4000)]
        [DataRow("KBD",    0x6000)]
        public void ReturnPredefinedSymbolAddresses(string input, int expectedOutput)
        {
            var binaryCodes = new BinaryCodes();
            Assert.AreEqual(expectedOutput, binaryCodes.Symb(input));
        }
    }
}
