
namespace VirtualMachine.Parsing
{
    public class Parser(IEnumerable<string> inputLines)
    {
        private IEnumerator<string> _enumerator = inputLines.GetEnumerator();

        public CommandType CommandType { get; private set; }
        public string Arg1 { get; private set; }
        public string Arg2 { get; private set; }

        public bool MoveNext()
        {
            if (!_enumerator.MoveNext())
                return false;

            var line = _enumerator.Current;

            var substrings = line.Split(' ');

            CommandType = _commandTypes[substrings[0]];

            if(substrings.Length == 1)
            {
                Arg1 = substrings[0];
                Arg2 = null;
            }
            else if (substrings.Length == 2)
            {
                Arg1 = substrings[1];
                Arg2 = null;
            }
            else if(substrings.Length == 3)
            {
                Arg1 = substrings[1];
                Arg2 = substrings[2];
            }

            return true;
        }

        private readonly Dictionary<string, CommandType> _commandTypes = new()
        {
            ["push"] = CommandType.C_PUSH,
            ["pop"] = CommandType.C_POP,

            ["add"] = CommandType.C_ARITHMETIC,
            ["sub"] = CommandType.C_ARITHMETIC,
            ["neg"] = CommandType.C_ARITHMETIC,
            ["eq"] = CommandType.C_ARITHMETIC,
            ["gt"] = CommandType.C_ARITHMETIC,
            ["lt"] = CommandType.C_ARITHMETIC,
            ["and"] = CommandType.C_ARITHMETIC,
            ["or"] = CommandType.C_ARITHMETIC,
            ["not"] = CommandType.C_ARITHMETIC,

            ["label"] = CommandType.C_LABEL,
            ["goto"] = CommandType.C_GOTO,
            ["if-goto"] = CommandType.C_IF,

            ["function"] = CommandType.C_FUNCTION,
            ["call"] = CommandType.C_CALL,
            ["return"] = CommandType.C_RETURN
        };
    }
}