
namespace Assembler.Code
{
    public class BinaryCodes
    {
        private readonly Dictionary<string, int> _destValues = new()
        {
            ["null"] = 0b000,
            ["M"] = 0b001,
            ["D"] = 0b010,
            ["MD"] = 0b011,
            ["A"] = 0b100,
            ["AM"] = 0b101,
            ["AD"] = 0b110,
            ["AMD"] = 0b111,
        };

        private readonly Dictionary<string, int> _jumpValues = new()
        {
            ["null"] = 0b000,
            ["JGT"] = 0b001,
            ["JEQ"] = 0b010,
            ["JGE"] = 0b011,
            ["JLT"] = 0b100,
            ["JNE"] = 0b101,
            ["JLE"] = 0b110,
            ["JMP"] = 0b111,
        };

        private readonly Dictionary<string, int> _compValues = new()
        {
            ["0"] = 0b0_101010,
            ["1"] = 0b0_111111,
            ["-1"] = 0b0_111010,
            ["D"] = 0b0_001100,
            ["A"] = 0b0_110000,
            ["!D"] = 0b0_001101,
            ["!A"] = 0b0_110001,
            ["-D"] = 0b0_001111,
            ["-A"] = 0b0_110011,
            ["D+1"] = 0b0_011111,
            ["A+1"] = 0b0_110111,
            ["D-1"] = 0b0_001110,
            ["A-1"] = 0b0_110010,
            ["D+A"] = 0b0_000010,
            ["D-A"] = 0b0_010011,
            ["A-D"] = 0b0_000111,
            ["D&A"] = 0b0_000000,
            ["D|A"] = 0b0_010101,
            ["M"] = 0b1_110000,
            ["!M"] = 0b1_110001,
            ["-M"] = 0b1_110011,
            ["M+1"] = 0b1_110111,
            ["M-1"] = 0b1_110010,
            ["D+M"] = 0b1_000010,
            ["D-M"] = 0b1_010011,
            ["M-D"] = 0b1_000111,
            ["D&M"] = 0b1_000000,
            ["D|M"] = 0b1_010101,
        };

        private readonly Dictionary<string, int> _predefinedSymbols = new()
        {
            ["SP"] =  0x0000,
            ["LCL"] = 0x0001,
            ["ARG"] = 0x0002,
            ["THIS"] =0x0003,
            ["THAT"] =0x0004,
            ["R0"] =  0x0000,
            ["R1"] =  0x0001,
            ["R2"] =  0x0002,
            ["R3"] =  0x0003,
            ["R4"] =  0x0004,
            ["R5"] =  0x0005,
            ["R6"] =  0x0006,
            ["R7"] =  0x0007,
            ["R8"] =  0x0008,
            ["R9"] =  0x0009,
            ["R10"] = 0x000A,
            ["R11"] = 0x000B,
            ["R12"] = 0x000C,
            ["R13"] = 0x000D,
            ["R14"] = 0x000E,
            ["R15"] = 0x000F,
            ["SCREEN"] = 0x4000,
            ["KBD"] = 0x6000,
        };

        public int Dest(string input)
        {
            return _destValues[input];
        }

        public int Jump(string input)
        {
            return _jumpValues[input];
        }

        public int Comp(string input)
        {
            return _compValues[input];
        }

        public int Symb(string input)
        {
            return _predefinedSymbols[input];
        }

        public Dictionary<string, int> GetPredefinedSymbols() => _predefinedSymbols;
    }
}