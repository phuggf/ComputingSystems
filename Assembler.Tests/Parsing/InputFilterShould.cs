using Assembler.Parsing;

namespace Assembler.Tests.Parsing
{
    [TestClass]
    public class InputFilterShould
    {
        [TestMethod]
        public void ReturnJustCommands()
        {
            var inputLines = new List<string>
            {
                "// File name: projects/06/max/Max.asm",
                "",
                "// Computes R2 = max(R0, R1)  (R0,R1,R2 refer to RAM[0],RAM[1],RAM[2])",
                "",
                "   @R0",
                "   D=M             // D = first number",
                "   @R1",
                "   D=D-M            // D = first number - second number",
                "   @OUTPUT_FIRST",
                "(OUTPUT_FIRST)",
            };

            var expectedOutput = new List<string>
            {
                "@R0",
                "D=M",
                "@R1",
                "D=D-M",
                "@OUTPUT_FIRST",
                "(OUTPUT_FIRST)"
            };

            var inputFilter = new InputFilter();
            var observedOutput = inputFilter.Filter(inputLines).ToArray();

            for (int i = 0; i < expectedOutput.Count; i++)
            {
                Assert.AreEqual(expectedOutput[i], observedOutput[i]);
            }
        }
    }
}
