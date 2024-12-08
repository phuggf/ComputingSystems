namespace Core
{
    public class InputFilter
    {
        public IEnumerable<string> Filter(IEnumerable<string> inputLines)
        {
            bool IsCommandLine(string inputLine)
            {
                if (string.IsNullOrWhiteSpace(inputLine))
                    return false;
                if (inputLine.StartsWith("//"))
                    return false;

                return true;
            }

            string TrimCommand(string inputLine)
            {
                var commentIndex = inputLine.IndexOf("//");

                if (commentIndex != -1)
                    inputLine = inputLine.Substring(0, commentIndex);

                return inputLine.Trim();
            }

            return inputLines.Where(IsCommandLine).Select(TrimCommand);
        }
    }
}