using Assembler.Code;
using Assembler.Parsing;

namespace Assembler
{
    public class Assembler
    {
        private const int RomAddressStart = 0;
        private const int FirstAvailableRamAddress = 16; // 0-15 are reserved for R0-R15

        private readonly BinaryCodes _binaryCodes = new();
        private readonly InputFilter _inputFilter = new();


        public IEnumerable<string> Assemble(IEnumerable<string> input)
        {
            var filteredLines = _inputFilter.Filter(input);
            var parser = new Parser(filteredLines);
            var symbolTable = new Dictionary<string, int>(_binaryCodes.GetPredefinedSymbols());
            var binaryFormatter = new BinaryFormatter(_binaryCodes);

            int romAddress = RomAddressStart;

            // First pass: populate the symbol table with the labels
            while (parser.MoveNext())
            {
                if (parser.CommandType == CommandType.L_Command)
                {
                    symbolTable.TryAdd(parser.Symbol, romAddress);
                }
                else
                {
                    romAddress++;
                }
            }

            parser.Reset();

            int nextVariableAddress = FirstAvailableRamAddress;

            // Second pass: convert the commands into binary values
            while (parser.MoveNext())
            {
                switch (parser.CommandType)
                {
                    case CommandType.A_Command:
                        if (int.TryParse(parser.Symbol, out int value))
                        {
                            yield return binaryFormatter.FormatACommand(value);
                        }
                        else if (symbolTable.TryGetValue(parser.Symbol, out int symbol))
                        {
                            yield return binaryFormatter.FormatACommand(symbol);
                        }
                        else
                        {
                            symbolTable.TryAdd(parser.Symbol, nextVariableAddress);
                            nextVariableAddress += 1;
                            yield return binaryFormatter.FormatACommand(symbolTable[parser.Symbol]);
                        }
                        break;
                    case CommandType.C_Command:
                        yield return binaryFormatter.FormatCCommand(parser.Comp, parser.Dest, parser.Jump);
                        break;
                    case CommandType.L_Command:
                        break;
                }
            }

        }

        private class BinaryFormatter(BinaryCodes binaryCodes)
        {
            private readonly BinaryCodes _binaryCodes = binaryCodes;

            public string FormatACommand(int value) => $"0{Symb(value)}";
            public string FormatCCommand(string comp, string dest, string jump)
                                                    => $"111{Comp(comp)}{Dest(dest)}{Jump(jump)}";

            private string Dest(string dest) => ToBinary(_binaryCodes.Dest(dest)).PadLeft(3, '0');
            private string Comp(string comp) => ToBinary(_binaryCodes.Comp(comp)).PadLeft(7, '0');
            private string Jump(string jump) => ToBinary(_binaryCodes.Jump(jump)).PadLeft(3, '0');
            private string Symb(int i) => ToBinary(i).PadLeft(15, '0');
            private static string ToBinary(int i) => Convert.ToString(i, 2);
        }
    }
}