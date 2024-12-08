using Core;
using VirtualMachine.Parsing;
using VirtualMachine.Tests.Code;

namespace VirtualMachine
{
    public class VmTranslator
    {
        private readonly InputFilter _inputFilter = new();

        public VmTranslator()
        {
        }

        public IEnumerable<string> ConvertToAssembly(IEnumerable<VmFile> vmFiles)
        {
            foreach (var vmFile in vmFiles)
            {
                var filteredLines = _inputFilter.Filter(vmFile.Contents);
                var parser = new Parser(filteredLines);
                var codeWriter = new CodeWriter();

                while(parser.MoveNext())
                {
                    switch (parser.CommandType)
                    {
                        case CommandType.C_ARITHMETIC:
                            foreach (var line in codeWriter.WriteArithmetic(parser.Arg1)) yield return line;
                            break;
                        case CommandType.C_PUSH:
                        case CommandType.C_POP:
                            foreach (var line in codeWriter.WritePushPop(parser.CommandType, parser.Arg1, int.Parse(parser.Arg2))) yield return line;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}