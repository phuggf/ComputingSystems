using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Tokenising
{
    public class Tokeniser(IEnumerable<string> inputLines)
    {
        private IEnumerator<string> _lineEnumerator = inputLines.GetEnumerator();
        private List<string> _currentTokens = [];
        private readonly HashSet<string> _keywords =
        [
            "class",
            "constructor",
            "function",
            "method",
            "field",
            "static",
            "var",
            "int",
            "char",
            "boolean",
            "void",
            "true",
            "false",
            "null",
            "this",
            "let",
            "do",
            "if",
            "else",
            "while",
            "return",
        ];

        private readonly HashSet<char> _symbols =
        [
            '{',
            '}',
            '(',
            ')',
            '[',
            ']',
            '.',
            ',',
            ';',
            '+',
            '-',
            '*',
            '/',
            '&',
            '|',
            '<',
            '>',
            '=',
            '~',
        ];

        public TokenType TokenType { get; private set; }
        public string Keyword { get; private set; }
        public string Symbol { get; private set; }
        public string Identifier { get; private set; }
        public int IntValue { get; private set; }
        public string StringValue { get; private set; }

        public bool MoveNext()
        {
            if (_currentTokens.Count == 0)
            {
                if (!_lineEnumerator.MoveNext())
                    return false;

                PopulateCurrentTokens();
            }

            var token = _currentTokens[0];

            if (_keywords.Contains(token))
            {
                TokenType = TokenType.KEYWORD;
                Keyword = token;
            }
            else if (_symbols.Contains(token[0]))
            {
                TokenType = TokenType.SYMBOL;
                Symbol = token;
            }
            else if (int.TryParse(token, out int value))
            {
                TokenType = TokenType.INT_CONST;
                IntValue = value;
            }
            else if (token.StartsWith('"') && token.EndsWith('"'))
            {
                TokenType = TokenType.STRING_CONST;
                StringValue = token.Substring(1, token.Length - 2);
            }
            else
            {
                TokenType = TokenType.IDENTIFIER;
                Identifier = token;
            }

            _currentTokens.RemoveAt(0);

            return true;
        }

        public string PeekSymbol()
        {
            if (_currentTokens.Count == 0)
            {
                if (!_lineEnumerator.MoveNext())
                {
                    return null;
                }

                PopulateCurrentTokens();
            }

            return _currentTokens[0];
        }

        public bool PeekKeyword()
        {
            if (_currentTokens.Count == 0)
            {
                if (!_lineEnumerator.MoveNext())
                {
                    return default;
                }

                PopulateCurrentTokens();
            }

            return _keywords.Contains(_currentTokens[0]);
        }

        private void PopulateCurrentTokens()
        {
            string line = _lineEnumerator.Current;
            var currentToken = new List<char>();
            bool insideQuote = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                {
                    currentToken = HandleSpace(currentToken, insideQuote);
                }
                else if (_symbols.Contains(line[i]))
                {
                    currentToken = HandleSymbol(line[i], currentToken);
                }
                else if (line[i] == '"')
                {
                    currentToken = HandleQuote(currentToken, insideQuote);
                    insideQuote = !insideQuote;
                }
                else
                {
                    currentToken.Add(line[i]);
                }
            }

            if (currentToken.Count > 0)
            {
                _currentTokens.Add(string.Concat(currentToken));
            }
        }

        private List<char> HandleQuote(List<char> currentToken, bool insideQuote)
        {
            currentToken.Add('"');

            if (insideQuote)
            {
                _currentTokens.Add(string.Concat(currentToken));
                currentToken = [];
            }

            return currentToken;
        }

        private List<char> HandleSymbol(char symbol, List<char> currentToken)
        {
            if (currentToken.Count != 0)
            {
                _currentTokens.Add(string.Concat(currentToken));
            }

            _currentTokens.Add(new string(symbol, 1));
            currentToken = [];
            return currentToken;
        }

        private List<char> HandleSpace(List<char> currentToken, bool insideQuote)
        {
            if (insideQuote)
            {
                currentToken.Add(' ');
            }
            else if (currentToken.Count > 0 && !insideQuote)
            {
                _currentTokens.Add(string.Concat(currentToken));
                currentToken = [];
            }

            return currentToken;
        }
    }
}
