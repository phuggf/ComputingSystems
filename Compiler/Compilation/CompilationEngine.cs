using Compiler.Tokenising;
using System.Linq.Expressions;
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

            if (_tokeniser.PeekKeyword())
            {
                AddKeyword(element);
            }
            else
            {
                AddIdentifier(element);
            }

            AddIdentifier(element);
            AddSymbol(element);
            element.Add(CompileParameterList());
            AddSymbol(element);

            var subroutineBody = new XElement("subroutineBody");
            AddSymbol(subroutineBody);

            while (_tokeniser.PeekSymbol() == "var")
            {
                subroutineBody.Add(CompileVarDec());
            }

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

            if (_tokeniser.PeekSymbol() == ";")
            {
                AddSymbol(element);
            }
            else
            {
                AddExpression(element);
                AddSymbol(element);
            }

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

            if (_tokeniser.PeekSymbol() == ".")
            {
                AddSymbol(element);
                AddIdentifier(element);
            }

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

            var ifStatements = CompileStatements();
            element.Add(ifStatements);

            AddSymbol(element);

            if (_tokeniser.PeekSymbol() == "else")
            {
                AddKeyword(element);
                AddSymbol(element);
                var elseStatements = CompileStatements();
                element.Add(elseStatements);
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

            var statements = CompileStatements();
            element.Add(statements);

            AddSymbol(element);
            return element;
        }

        public XElement CompileExpression() 
        {
            var expression = new XElement("expression");

            if(_tokeniser.PeekSymbol() == "(")
            {
                var term = new XElement("term");
                AddSymbol(term);
                term.Add(CompileExpression());
                AddSymbol(term);
                expression.Add(term);
            }

            while( !IsExpressionEnd(_tokeniser.PeekSymbol()))
            {
                _tokeniser.MoveNext();
                switch (_tokeniser.TokenType)
                {
                    case TokenType.KEYWORD:
                        var keywTerm = new XElement("term");
                        keywTerm.Add(new XElement("keyword", _tokeniser.Keyword));
                        expression.Add(keywTerm);
                        break;
                    case TokenType.SYMBOL:
                        expression.Add(new XElement("symbol", _tokeniser.Symbol));
                        break;
                    case TokenType.IDENTIFIER:
                        var idenTerm = new XElement("term");
                        idenTerm.Add(new XElement("identifier", _tokeniser.Identifier));
                        expression.Add(idenTerm);
                        break;
                    case TokenType.INT_CONST:
                        var intTerm = new XElement("term");
                        intTerm.Add(new XElement("integerConstant", _tokeniser.IntValue));
                        expression.Add(intTerm);
                        break;
                    case TokenType.STRING_CONST:
                        var strTerm = new XElement("term");
                        strTerm.Add(new XElement("stringConstant", _tokeniser.StringValue));
                        expression.Add(strTerm);
                        break;
                }
            }

            return expression;
        }

        public XElement CompileTerm()
        {
            var term = new XElement("term");

            _tokeniser.MoveNext();
            switch (_tokeniser.TokenType)
            {
                case TokenType.KEYWORD:
                    term.Add(new XElement("keyword", _tokeniser.Keyword));
                    return term;
                case TokenType.SYMBOL:
                    term.Add(new XElement("symbol", _tokeniser.Symbol));
                    return term;
                case TokenType.IDENTIFIER:
                    term.Add(new XElement("identifier", _tokeniser.Identifier));
                    if(_tokeniser.PeekSymbol() == ".")
                    {
                        AddSymbol(term);
                        AddIdentifier(term);
                        AddSymbol(term);
                        term.Add(new XElement("expressionList", string.Empty));
                        AddSymbol(term);
                    }
                    else if( _tokeniser.PeekSymbol() == "[")
                    {
                        AddSymbol(term);
                        term.Add(CompileExpression());
                        AddSymbol(term);
                    }
                    return term;
                case TokenType.INT_CONST:
                    term.Add(new XElement("integerConstant", _tokeniser.IntValue));
                    return term;
                case TokenType.STRING_CONST:
                    term.Add(new XElement("stringConstant", _tokeniser.StringValue));
                    return term;
            }

            return term;

            //while (_tokeniser.PeekSymbol() != ";")
            //{
            //    _tokeniser.MoveNext();
            //    switch (_tokeniser.TokenType)
            //    {
            //        case TokenType.KEYWORD:
            //            term.Add(new XElement("keyword", _tokeniser.Keyword));
            //            expression.Add(keywTerm);
            //            break;
            //        case TokenType.SYMBOL:
            //            expression.Add(new XElement("symbol", _tokeniser.Symbol));
            //            break;
            //        case TokenType.IDENTIFIER:
            //            var idenTerm = new XElement("term");
            //            idenTerm.Add(new XElement("identifier", _tokeniser.Identifier));
            //            expression.Add(idenTerm);
            //            break;
            //        case TokenType.INT_CONST:
            //            var intTerm = new XElement("term");
            //            intTerm.Add(new XElement("integerConstant", _tokeniser.IntValue));
            //            expression.Add(intTerm);
            //            break;
            //        case TokenType.STRING_CONST:
            //            var strTerm = new XElement("term");
            //            strTerm.Add(new XElement("stringConstant", _tokeniser.StringValue));
            //            expression.Add(strTerm);
            //            break;
            //    }
            //}
        }

        private void AddExpression(XElement element)
        {
            var expression = new XElement("expression");
            var term = new XElement("term");

            if (_tokeniser.PeekKeyword())
            {
                AddKeyword(term);
            }
            else
            {
                AddIdentifier(term);
            }

            expression.Add(term);
            element.Add(expression);
        }

        private void AddExpressionList(XElement element)
        {
            var expressionList = new XElement("expressionList", string.Empty);

            if(_tokeniser.PeekSymbol() != ")")
            {
                AddExpression(expressionList);
            }

            while(_tokeniser.PeekSymbol() != ")")
            {
                AddSymbol(expressionList);
                AddExpression(expressionList);
            }

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
        private bool IsSubroutine(string name) => name == "method" ||
                                                  name == "function" ||
                                                  name == "constructor";

        private bool IsExpressionEnd(string name) => name == ")" ||
                                                     name == "]";

    }
}
