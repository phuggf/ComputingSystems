using VirtualMachine.Code;
using VirtualMachine.Parsing;

namespace VirtualMachine.Tests.Code
{
    public class CodeWriter(string fileName)
    {
        private ArithmeticWriter _arithmeticWriter = new(fileName);
        private PushPopWriter _pushPopWriter = new(fileName);

        public string CurrentVmFile { get; internal set; }

        public IEnumerable<string> WritePushPop(CommandType type, string segment, int index)
        {
            return _pushPopWriter.WritePushPop(type, segment, index);
        }

        public IEnumerable<string> WriteArithmetic(string command, string currentFunction = null)
        {
            return _arithmeticWriter.GetCommands(command, currentFunction);
        }
    }
}