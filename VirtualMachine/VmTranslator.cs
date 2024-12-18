using Core;
using System.Globalization;
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
            var codeWriter = new CodeWriter();

            foreach (var line in codeWriter.WriteInit())
            {
                yield return line;
            }

            foreach (var vmFile in vmFiles)
            {
                var filteredLines = _inputFilter.Filter(vmFile.Contents);
                var parser = new Parser(filteredLines);

                while (parser.MoveNext())
                {
                    switch (parser.CommandType)
                    {
                        case CommandType.C_ARITHMETIC:
                            foreach (var line in codeWriter.WriteArithmetic(parser.Arg1)) yield return line;
                            break;
                        case CommandType.C_PUSH:
                            foreach (var line in codeWriter.WritePush(parser.Arg1, int.Parse(parser.Arg2))) yield return line;
                            break;
                        case CommandType.C_POP:
                            foreach (var line in codeWriter.WritePop(parser.Arg1, int.Parse(parser.Arg2))) yield return line;
                            break;
                        case CommandType.C_LABEL:
                            foreach (var line in codeWriter.WriteLabel(parser.Arg1)) yield return line;
                            break;
                        case CommandType.C_GOTO:
                            foreach (var line in codeWriter.WriteGoto(parser.Arg1)) yield return line;
                            break;
                        case CommandType.C_IF:
                            foreach (var line in codeWriter.WriteIf(parser.Arg1)) yield return line;
                            break;
                        case CommandType.C_FUNCTION:
                            foreach (var line in codeWriter.WriteFunction(parser.Arg1, int.Parse(parser.Arg2))) yield return line;
                            break;
                        case CommandType.C_RETURN:
                            foreach (var line in codeWriter.WriteReturn()) yield return line;
                            break;
                        case CommandType.C_CALL:
                            foreach (var line in codeWriter.WriteCall(parser.Arg1, int.Parse(parser.Arg2))) yield return line;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}