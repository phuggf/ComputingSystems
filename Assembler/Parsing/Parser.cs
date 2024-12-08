
namespace Assembler.Parsing
{
    public class Parser(IEnumerable<string> inputLines)
    {
        private IEnumerator<string> _enumerator = inputLines.GetEnumerator();
        private readonly IEnumerable<string> _inputLines = inputLines;
        private Command _currentCommand;

        public CommandType CommandType => _currentCommand.CommandType;
        public string Symbol => _currentCommand.Symbol;
        public string Dest => _currentCommand.Dest;
        public string Comp => _currentCommand.Comp;
        public string Jump => _currentCommand.Jump;

        public bool MoveNext()
        {
            if (!_enumerator.MoveNext())
                return false;

            var line = _enumerator.Current;

            _currentCommand = Command.New(line);

            return true;
        }

        public void Reset()
        {
            // apparently not recommended to call Reset on base enumerator as
            // behaviour can differ depending on underlying collection, so better
            // to call GetEnumerator again
            _enumerator = _inputLines.GetEnumerator();
        }

        private abstract class Command
        {
            public static Command New(string line)
            {
                if (line.StartsWith("@"))
                    return new ACommand(line);
                else if (line.StartsWith("("))
                    return new LCommand(line);
                else
                    return new CCommand(line);
            }

            public abstract CommandType CommandType { get; }
            public string Symbol { get; protected set; } = null;
            public string Dest { get; protected set; } = null;
            public string Comp { get; internal set; } = null;
            public string Jump { get; internal set; } = null;
        }

        private class ACommand : Command
        {
            public ACommand(string commandLine) => Symbol = commandLine.Substring(1);

            public override CommandType CommandType => CommandType.A_Command;
        }

        private class CCommand : Command
        {
            public CCommand(string commandLine)
            {
                var equalsIndex = commandLine.IndexOf('=');
                var semicolonIndex = commandLine.IndexOf(';');

                if (equalsIndex != -1 && semicolonIndex != -1)
                {
                    Dest = commandLine.Substring(0, equalsIndex);
                    Comp = commandLine.Substring(equalsIndex + 1, semicolonIndex - equalsIndex);
                    Jump = commandLine.Substring(semicolonIndex + 1);
                }
                else if (equalsIndex != -1 && semicolonIndex == -1)
                {
                    Dest = commandLine.Substring(0, equalsIndex);
                    Comp = commandLine.Substring(equalsIndex + 1);
                    Jump = "null";
                }
                else
                {
                    Dest = "null";
                    Comp = commandLine.Substring(0, semicolonIndex);
                    Jump = commandLine.Substring(semicolonIndex + 1);
                }
            }

            public override CommandType CommandType => CommandType.C_Command;

        }

        private class LCommand : Command
        {
            public LCommand(string commandLine) => Symbol = commandLine.Substring(1, commandLine.Length - 2);

            public override CommandType CommandType => CommandType.L_Command;
        }


    }
}