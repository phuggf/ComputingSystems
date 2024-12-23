namespace Core
{
    public class InputFilter
    {
        private bool _isCommentedOut = false;

        public IEnumerable<string> Filter(IEnumerable<string> inputLines)
        {
            return inputLines.Select(TrimLine).Where(line => !string.IsNullOrWhiteSpace(line));
        }

        private string TrimLine(string inputLine)
        {
            var lineCommentIndex = inputLine.IndexOf("//");
            var commentStartIndex = inputLine.IndexOf("/*");
            var commentEndIndex = inputLine.IndexOf("*/");
            
            if (lineCommentIndex != -1)
            {
                inputLine = inputLine.Substring(0, lineCommentIndex);
            }
            else if (commentStartIndex != -1 && commentEndIndex != -1)
            {
                inputLine = inputLine.Substring(0, commentStartIndex) +
                            inputLine.Substring(commentEndIndex + 2);
            }
            else if (commentStartIndex != -1)
            {
                inputLine = inputLine.Substring(0, commentStartIndex);
                _isCommentedOut = true;
            }
            else if (commentEndIndex != -1)
            {
                inputLine = inputLine.Substring(commentEndIndex + 2);
                _isCommentedOut = false;
            }

            if (_isCommentedOut && commentStartIndex == -1 )
            {
                return string.Empty;
            }

            return inputLine.Replace("  ", " ").Trim();
        }
    }
}