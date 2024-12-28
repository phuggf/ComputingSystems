using Compiler.Tokenising;
using System.Xml.Linq;

namespace Compiler.Compilation
{
    public class CompilationEngine(Tokeniser tokeniser)
    {
        private Tokeniser _tokeniser = tokeniser;

        public XElement CompileClass()
        {
            var classElement = new XElement("class");
            AddKeyword(classElement);
            AddIdentifier(classElement);
            AddSymbol(classElement);

            while (IsClassVarDec(_tokeniser.PeekSymbol()))
            {
                classElement.Add(CompileClassVarDec());
            }

            while (IsSubroutine(_tokeniser.PeekSymbol()))
            {
                classElement.Add(CompileSubroutine());
            }

            AddSymbol(classElement);
            return classElement;
        }

        public XElement CompileClassVarDec()
        {
            var element = new XElement("classVarDec");
            AddVarDec(element);
            return element;
        }

        public XElement CompileSubroutine()
        {
            var element = new XElement("subroutineDec");
            AddKeyword(element);
            AddKeyword(element);
            AddIdentifier(element);
            AddSymbol(element);
            element.Add(CompileParameterList());
            AddSymbol(element);

            var subroutineBody = new XElement("subroutineBody");
            AddSymbol(subroutineBody);
            subroutineBody.Add(CompileVarDec());
            subroutineBody.Add(CompileStatements());
            AddSymbol(subroutineBody);
            element.Add(subroutineBody);

            return element;
        }

        public XElement CompileParameterList()
        {
            var element = new XElement("parameterList", string.Empty);

            if (_tokeniser.PeekSymbol() == ")")
            {
                return element;
            }

            AddKeyword(element);
            AddIdentifier(element);

            while (_tokeniser.PeekSymbol() != ")")
            {
                AddSymbol(element);
                AddKeyword(element);
                AddIdentifier(element);
            }

            return element;
        }

        public XElement CompileVarDec()
        {
            var element = new XElement("varDec");
            AddVarDec(element);
            return element;
        }

        public XElement CompileStatements()
        {
            var element = new XElement("statements", string.Empty);

            var statementType = _tokeniser.PeekSymbol();

            while (IsStatement(statementType))
            {
                switch (statementType)
                {
                    case "do":
                        element.Add(CompileDoStatement()); break;
                    case "while":
                        element.Add(CompileWhileStatement()); break;
                    case "if":
                        element.Add(CompileIfStatement()); break;
                    case "let":
                        element.Add(CompileLetStatement()); break;
                    case "return":
                        element.Add(CompileReturnStatement()); break;
                }

                statementType = _tokeniser.PeekSymbol();
            }

            return element;
        }

        public XElement CompileReturnStatement()
        {
            var element = new XElement("returnStatement");
            AddKeyword(element);
            AddSymbol(element);
            return element;
        }

        public XElement CompileLetStatement()
        {
            var element = new XElement("letStatement");
            AddKeyword(element);
            AddIdentifier(element);
            AddSymbol(element);
            AddExpression(element);
            AddSymbol(element);
            return element;
        }


        public XElement CompileDoStatement()
        {
            var element = new XElement("doStatement");
            AddKeyword(element);
            AddIdentifier(element);
            AddSymbol(element);
            AddIdentifier(element);
            AddSymbol(element);
            AddExpressionList(element);
            AddSymbol(element);
            AddSymbol(element);
            return element;
        }

        public XElement CompileIfStatement()
        {
            var element = new XElement("ifStatement");
            AddKeyword(element);
            AddSymbol(element);
            AddExpression(element);
            AddSymbol(element);
            AddSymbol(element);
            AddStatements(element);
            AddSymbol(element);

            if (_tokeniser.PeekSymbol() == "else")
            {
                AddKeyword(element);
                AddSymbol(element);
                AddStatements(element);
                AddSymbol(element);
            }

            return element;
        }

        public XElement CompileWhileStatement()
        {
            var element = new XElement("whileStatement");
            AddKeyword(element);
            AddSymbol(element);
            AddExpression(element);
            AddSymbol(element);
            AddSymbol(element);
            AddStatements(element);
            AddSymbol(element);
            return element;
        }

        private void AddStatements(XElement element)
        {
            var statements = new XElement("statements", string.Empty);
            element.Add(statements);
        }

        private void AddExpression(XElement element)
        {
            var expression = new XElement("expression");
            var term = new XElement("term");
            AddIdentifier(term);
            expression.Add(term);
            element.Add(expression);
        }

        private void AddExpressionList(XElement element)
        {
            var expressionList = new XElement("expressionList", string.Empty);
            element.Add(expressionList);
        }

        private void AddVarDec(XElement element)
        {
            AddKeyword(element);

            if (_tokeniser.PeekKeyword())
            {
                AddKeyword(element);
            }
            else
            {
                AddIdentifier(element);
            }

            AddIdentifier(element);

            while (_tokeniser.PeekSymbol() != ";")
            {
                AddSymbol(element);
                AddIdentifier(element);
            }

            AddSymbol(element);
        }

        private void AddKeyword(XElement element)
        {
            _tokeniser.MoveNext();
            element.Add(new XElement("keyword", _tokeniser.Keyword));
        }

        private void AddIdentifier(XElement element)
        {
            _tokeniser.MoveNext();
            element.Add(new XElement("identifier", _tokeniser.Identifier));
        }

        private void AddSymbol(XElement element)
        {
            _tokeniser.MoveNext();
            element.Add(new XElement("symbol", _tokeniser.Symbol));
        }

        private bool IsClassVarDec(string name) => name == "static" || name == "field";
        private bool IsStatement(string name) => name == "do" ||
                                                 name == "let" ||
                                                 name == "while" ||
                                                 name == "if" ||
                                                 name == "return";
        private bool IsSubroutine(string name) => name == "method" || name == "function";

    }
}
