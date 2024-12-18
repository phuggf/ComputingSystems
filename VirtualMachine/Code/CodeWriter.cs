using System.Reflection.Emit;
using System.Xml.Serialization;
using VirtualMachine.Code;
using VirtualMachine.Parsing;

namespace VirtualMachine.Tests.Code
{
    public class CodeWriter()
    {
        private readonly ArithmeticWriter _arithmeticWriter = new();
        private readonly StackWriter _stackWriter = new();
        private int _returnAddressCounter = 0;
        private string _currentFunction;

        public string CurrentVmFile { get; internal set; }

        public IEnumerable<string> WritePush(string segment, int index)
        {
            return _stackWriter.Push(segment, index);
        }

        public IEnumerable<string> WritePop(string segment, int index)
        {
            return _stackWriter.Pop(segment, index);
        }

        public IEnumerable<string> WriteArithmetic(string command, string currentFunction = null)
        {
            return _arithmeticWriter.GetCommands(command, _currentFunction);
        }

        public IEnumerable<string> WriteInit()
        {
            yield return "@256";
            yield return "D=A";
            yield return "@SP";
            yield return "M=D";

            foreach( var line in WriteCall("Sys.init", 0))
            {
                yield return line;
            }
        }

        public IEnumerable<string> WriteCall(string name, int numArgs)
        {
            var ret = new List<string>();
            string returnAddress = $"return-address{_returnAddressCounter}";
            int argSubtractor = numArgs + 5;

            ret.AddRange(_stackWriter.PushConstant(returnAddress));
            ret.AddRange(_stackWriter.PushVariable("LCL"));
            ret.AddRange(_stackWriter.PushVariable("ARG"));
            ret.AddRange(_stackWriter.PushVariable("THIS"));
            ret.AddRange(_stackWriter.PushVariable("THAT"));
            ret.AddRange(
            [
                $"@{argSubtractor}",
                "D=A",
                "@SP",
                "D=M-D",
                "@ARG",
                "M=D", // ARG = SP-n-5
                "@SP",
                "D=M",
                "@LCL",
                "M=D", // LCL = SP
                $"@{name}",
                "0;JMP", // Goto name
                $"({returnAddress})",
            ]);
            _returnAddressCounter++;

            return ret;
        }

        public IEnumerable<string> WriteFunction(string name, int numLocals)
        {
            string label = $"({name})";
            var initialiseLocals = new List<string>() { label };

            for (int i = 0; i < numLocals; i++)
            {
                initialiseLocals.AddRange(_stackWriter.Push("constant", 0));
            }

            _currentFunction = name;

            return initialiseLocals;
        }

        public IEnumerable<string> WriteLabel(string label)
        {
            if (_currentFunction == null)
            {
                return [$"({label})"];
            }
            else
            {
                return [$"({_currentFunction}${label})"];
            }
        }

        public IEnumerable<string> WriteGoto(string label)
        {
            return [$"@{(_currentFunction == null ? label : $"{_currentFunction}${label}")}", "0;JMP"];
        }

        public IEnumerable<string> WriteIf(string label)
        {
            return
            [
                "@SP",
                "AM=M-1",
                "D=M",
                $"@{(_currentFunction == null ? label : $"{_currentFunction}${label}")}",
                "D;JLT",
            ];
        }

        public IEnumerable<string> WriteReturn()
        {
            return
            [
                "@LCL",
                "D=M",
                "@R13",
                "M=D", // FRAME = LCL
                "@5",
                "A=D-A",
                "D=M",
                "@R14",
                "M=D", // RET = *(FRAME-5)
                "@SP",
                "AM=M-1",
                "D=M",
                "@ARG",
                "A=M",
                "M=D", // *ARG = pop()
                "@ARG",
                "D=M+1",
                "@SP",
                "M=D", // SP = ARG+1
                "@R13",
                "AM=M-1",
                "D=M",
                "@THAT",
                "M=D", // THAT = *(FRAME-1)
                "@R13",
                "AM=M-1",
                "D=M",
                "@THIS",
                "M=D", // THIS = *(FRAME-2)
                "@R13",
                "AM=M-1",
                "D=M",
                "@ARG",
                "M=D", // ARG = *(FRAME-3)
                "@R13",
                "AM=M-1",
                "D=M",
                "@LCL",
                "M=D", // LCL = *(FRAME-4)
                "@R14",
                "A=M",
                "0;JMP", // goto RET
            ];
        }
    }
}