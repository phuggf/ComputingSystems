using VirtualMachine.Parsing;

namespace VirtualMachine.Code
{
    internal class PushPopWriter(string currentVmFile)
    {
        private string _currentVmFile = currentVmFile;

        public IEnumerable<string> WritePushPop(CommandType type, string segment, int index)
        {
            if (type == CommandType.C_PUSH)
            {
                return segment switch
                {
                    "constant" => PushConstant(index),
                    "local" => PushSegment("LCL", index),
                    "argument" => PushSegment("ARG", index),
                    "this" => PushSegment("THIS", index),
                    "that" => PushSegment("THAT", index),
                    "temp" => PushVariable($"R{index + 5}"),
                    "pointer" => PushVariable($"R{index + 3}"),
                    "static" => PushVariable($"{_currentVmFile}.{index}"),
                _ => throw new ArgumentException($"Unknown segment {segment}"),
                };
            }
            else if (type == CommandType.C_POP)
            {
                return segment switch
                {
                    "local" => PopSegment("LCL", index),
                    "argument" => PopSegment("ARG", index),
                    "this" => PopSegment("THIS", index),
                    "that" => PopSegment("THAT", index),
                    "temp" => PopVariable($"R{index + 5}"),
                    "pointer" => PopVariable($"R{index + 3}"),
                    "static" => PopVariable($"{_currentVmFile}.{index}"),
                    _ => throw new ArgumentException($"Unknown segment {segment}"),
                };
            }
            else
            {
                throw new ArgumentException($"{type} not allowed");
            }
        }

        private IEnumerable<string> PushConstant(int value)
        {
            return [$"@{value}", "D=A", "@SP", "M=M+1", "A=M-1", "M=D"];
        }

        private IEnumerable<string> PushConstant(string value)
        {
            return [$"@{value}", "D=A", "@SP", "M=M+1", "A=M-1", "M=D"];
        }

        private IEnumerable<string> PushVariable(string variable)
        {
            return [$"@{variable}", "D=M", "@SP", "M=M+1", "A=M-1", "M=D"];
        }

        private IEnumerable<string> PopVariable(string variable)
        {
            return ["@SP", "AM=M-1", "D=M", $"@{variable}", "M=D"];
        }

        private IEnumerable<string> PushSegment(string register, int value)
        {
            return [$"@{register}", "D=M", $"@{value}", "A=D+A", "D=M", "@SP", "M=M+1", "A=M-1", "M=D"];
        }

        private IEnumerable<string> PopSegment(string register, int value)
        {
            return [$"@{register}", "D=M", $"@{value}", "D=D+A", "@R13", "M=D", "@SP", "AM=M-1", "D=M", "@R13", "A=M", "M=D"];
        }
    }
}