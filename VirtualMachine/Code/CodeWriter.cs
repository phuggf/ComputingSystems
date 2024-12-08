using VirtualMachine.Code;
using VirtualMachine.Parsing;

namespace VirtualMachine.Tests.Code
{
    public class CodeWriter
    {
        private ArithmeticWriter _arithmeticWriter = new();
        private PushPopWriter _pushPopWriter = new();

        public CodeWriter()
        {
        }

        public IEnumerable<string> WritePushPop(CommandType type, string segment, int index)
        {
            return _pushPopWriter.WritePushPop(type, segment, index);
        }

        public IEnumerable<string> WriteArithmetic(string command)
        {
            return _arithmeticWriter.GetCommands(command, null);//_currentFunction);
        }
    }
}