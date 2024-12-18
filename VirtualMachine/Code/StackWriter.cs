using VirtualMachine.Parsing;

namespace VirtualMachine.Code
{
    internal class StackWriter()
    {
        public string CurrentVmFile { get; set; }

        public IEnumerable<string> Push( string segment, int index)
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
                "static" => PushVariable($"{CurrentVmFile}.{index}"),
                _ => throw new ArgumentException($"Unknown segment {segment}"),
            };
        }

        public IEnumerable<string> Pop(string segment, int index)
        {
            return segment switch
            {
                "local" => PopSegment("LCL", index),
                "argument" => PopSegment("ARG", index),
                "this" => PopSegment("THIS", index),
                "that" => PopSegment("THAT", index),
                "temp" => PopVariable($"R{index + 5}"),
                "pointer" => PopVariable($"R{index + 3}"),
                "static" => PopVariable($"{CurrentVmFile}.{index}"),
                _ => throw new ArgumentException($"Unknown segment {segment}"),
            };
        }

        public IEnumerable<string> PushConstant(int value)
        {
            return [$"@{value}", "D=A", "@SP", "M=M+1", "A=M-1", "M=D"];
        }

        public IEnumerable<string> PushConstant(string value)
        {
            return [$"@{value}", "D=A", "@SP", "M=M+1", "A=M-1", "M=D"];
        }

        public IEnumerable<string> PushVariable(string variable)
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