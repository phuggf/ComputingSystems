using Compiler.Compilation;
using Compiler.Tokenising;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Compiler.Tests.Compilation
{
    [TestClass]
    public class CompilationEngineShould
    {
        private const string Iden = "identifier";
        private const string Keyw = "keyword";
        private const string Symb = "symbol";

        [TestMethod]
        public void CompileEmptyClass()
        {
            var input = new string[] { "class Main {}" };

            var classElement = new XElement("class");
            classElement.Add(new XElement(Keyw, "class"));
            classElement.Add(new XElement(Iden, "Main"));
            classElement.Add(new XElement(Symb, "{"));
            classElement.Add(new XElement(Symb, "}"));

            var expectedOutput = classElement.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileClass();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileClassVarDecSingle()
        {
            var input = new string[] { "static ClassName test;" };

            var element = new XElement("classVarDec");
            element.Add(new XElement(Keyw, "static"));
            element.Add(new XElement(Iden, "ClassName"));
            element.Add(new XElement(Iden, "test"));
            element.Add(new XElement(Symb, ";"));

            var expectedOutput = element.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileClassVarDec();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileClassVarDecMultiple()
        {
            var input = new string[] { "field int x, y;" };

            var element = new XElement("classVarDec");
            element.Add(new XElement(Keyw, "field"));
            element.Add(new XElement(Keyw, "int"));
            element.Add(new XElement(Iden, "x"));
            element.Add(new XElement(Symb, ","));
            element.Add(new XElement(Iden, "y"));
            element.Add(new XElement(Symb, ";"));

            var expectedOutput = element.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileClassVarDec();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileSubroutine()
        {
            var input = new string[]
            {
                "function void main() {",
                "var SquareGame game;",
                "return;",
                "}",
            };

            var element = new XElement("subroutineDec");
            element.Add(new XElement(Keyw, "function"));
            element.Add(new XElement(Keyw, "void"));
            element.Add(new XElement(Iden, "main"));
            element.Add(new XElement(Symb, "("));

            var parameterList = new XElement("parameterList", string.Empty);
            element.Add(parameterList);

            element.Add(new XElement(Symb, ")"));

            var subroutineBody = new XElement("subroutineBody");
            element.Add(subroutineBody);

            subroutineBody.Add(new XElement(Symb, "{"));

            var varDec = new XElement("varDec");
            varDec.Add(new XElement(Keyw, "var"));
            varDec.Add(new XElement(Iden, "SquareGame"));
            varDec.Add(new XElement(Iden, "game"));
            varDec.Add(new XElement(Symb, ";"));
            subroutineBody.Add(varDec);

            var statements = new XElement("statements");
            subroutineBody.Add(statements);

            var retStatement = new XElement("returnStatement");
            retStatement.Add(new XElement(Keyw, "return"));
            retStatement.Add(new XElement(Symb, ";"));
            statements.Add(retStatement);

            subroutineBody.Add(new XElement(Symb, "}"));

            var expectedOutput = element.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileSubroutine();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileEmptyParameterList()
        {
            var input = new string[] { ")" };

            var element = new XElement("parameterList", string.Empty);

            var expectedOutput = element.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileParameterList();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileParameterListSingle()
        {
            var input = new string[] { "int x)" };

            var element = new XElement("parameterList");
            element.Add(new XElement(Keyw, "int"));
            element.Add(new XElement(Iden, "x"));

            var expectedOutput = element.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileParameterList();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileParameterListMultiple()
        {
            var input = new string[] { "int x, int y)" };

            var element = new XElement("parameterList");
            element.Add(new XElement(Keyw, "int"));
            element.Add(new XElement(Iden, "x"));
            element.Add(new XElement(Symb, ","));
            element.Add(new XElement(Keyw, "int"));
            element.Add(new XElement(Iden, "y"));

            var expectedOutput = element.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileParameterList();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileVarDecSingle()
        {
            var input = new string[] { "var String s;" };

            var element = new XElement("varDec");
            element.Add(new XElement(Keyw, "var"));
            element.Add(new XElement(Iden, "String"));
            element.Add(new XElement(Iden, "s"));
            element.Add(new XElement(Symb, ";"));

            var expectedOutput = element.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileVarDec();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileVarDecMultiple()
        {
            var input = new string[] { "var int i, j;" };

            var element = new XElement("varDec");
            element.Add(new XElement(Keyw, "var"));
            element.Add(new XElement(Keyw, "int"));
            element.Add(new XElement(Iden, "i"));
            element.Add(new XElement(Symb, ","));
            element.Add(new XElement(Iden, "j"));
            element.Add(new XElement(Symb, ";"));

            var expectedOutput = element.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileVarDec();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileLetStatement()
        {
            var input = new string[] { "let game = game;", };

            var letElement = new XElement("letStatement");
            letElement.Add(new XElement(Keyw, "let"));
            letElement.Add(new XElement(Iden, "game"));
            letElement.Add(new XElement(Symb, "="));

            var expression = new XElement("expression");
            var term = new XElement("term");
            term.Add(new XElement(Iden, "game"));
            expression.Add(term);
            letElement.Add(expression);

            letElement.Add(new XElement(Symb, ";"));

            var expectedOutput = letElement.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileLetStatement();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileDoStatement()
        {
            var input = new string[] { "do game.run();", };

            var letElement = new XElement("doStatement");
            letElement.Add(new XElement(Keyw, "do"));
            letElement.Add(new XElement(Iden, "game"));
            letElement.Add(new XElement(Symb, "."));
            letElement.Add(new XElement(Iden, "run"));
            letElement.Add(new XElement(Symb, "("));

            var expressionList = new XElement("expressionList", string.Empty);
            letElement.Add(expressionList);

            letElement.Add(new XElement(Symb, ")"));
            letElement.Add(new XElement(Symb, ";"));

            var expectedOutput = letElement.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileDoStatement();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileReturnStatement()
        {
            var input = new string[] { "return;", };

            var retStatement = new XElement("returnStatement");
            retStatement.Add(new XElement(Keyw, "return"));
            retStatement.Add(new XElement(Symb, ";"));

            var expectedOutput = retStatement.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileReturnStatement();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileIfStatement()
        {
            var input = new string[] { "if (a) {",
            "}"};

            var letElement = new XElement("ifStatement");
            letElement.Add(new XElement(Keyw, "if"));
            letElement.Add(new XElement(Symb, "("));

            var expression = new XElement("expression");
            var term = new XElement("term");
            term.Add(new XElement(Iden, "a"));
            expression.Add(term);
            letElement.Add(expression);

            letElement.Add(new XElement(Symb, ")"));
            letElement.Add(new XElement(Symb, "{"));

            var statements = new XElement("statements");
            letElement.Add(statements);

            letElement.Add(new XElement(Symb, "}"));

            var expectedOutput = letElement.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileIfStatement();

            Assert.AreEqual(expectedOutput, result.ToString());
        }

        [TestMethod]
        public void CompileIfElseStatement()
        {
            var input = new string[] 
            { 
                "if (a) {",
                "}",
                "else {",
                "}"
            };

            var letElement = new XElement("ifStatement");
            letElement.Add(new XElement(Keyw, "if"));
            letElement.Add(new XElement(Symb, "("));

            var expression = new XElement("expression");
            var term = new XElement("term");
            term.Add(new XElement(Iden, "a"));
            expression.Add(term);
            letElement.Add(expression);

            letElement.Add(new XElement(Symb, ")"));
            letElement.Add(new XElement(Symb, "{"));

            var ifStatements = new XElement("statements");
            letElement.Add(ifStatements);

            letElement.Add(new XElement(Symb, "}"));

            letElement.Add(new XElement(Keyw, "else"));
            letElement.Add(new XElement(Symb, "{"));

            var elseStatements = new XElement("statements");
            letElement.Add(ifStatements);

            letElement.Add(new XElement(Symb, "}"));

            var expectedOutput = letElement.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileIfStatement();

            Assert.AreEqual(expectedOutput, result.ToString());
        }


        [TestMethod]
        public void CompileWhileStatement()
        {
            var input = new string[]
            { 
                "while (a) {",
                "}"
            };

            var letElement = new XElement("whileStatement");
            letElement.Add(new XElement(Keyw, "while"));
            letElement.Add(new XElement(Symb, "("));

            var expression = new XElement("expression");
            var term = new XElement("term");
            term.Add(new XElement(Iden, "a"));
            expression.Add(term);
            letElement.Add(expression);

            letElement.Add(new XElement(Symb, ")"));
            letElement.Add(new XElement(Symb, "{"));

            var statements = new XElement("statements");
            letElement.Add(statements);

            letElement.Add(new XElement(Symb, "}"));

            var expectedOutput = letElement.ToString();

            var tokeniser = new Tokeniser(input);
            var engine = new CompilationEngine(tokeniser);
            XElement result = engine.CompileWhileStatement();

            Assert.AreEqual(expectedOutput, result.ToString());
        }


        //[TestMethod]
        //public void CompileStatementSequence()
        //{
        //    var input = new string[]
        //    {
        //        "let game = game",
        //        "do game.run();",
        //        "while (a) {",
        //        "}"
        //    };

        //    var letElement = new XElement("letStatement");
        //    letElement.Add(new XElement("keyword", "let"));
        //    letElement.Add(new XElement("identifier", "game"));
        //    letElement.Add(new XElement("symbol", "="));

        //    var expression = new XElement("expression");
        //    var term = new XElement("term");
        //    term.Add(new XElement("idetifier", "game"));
        //    expression.Add(term);
        //    letElement.Add(expression);

        //    letElement.Add(new XElement("symbol", ";"));

        //    var expectedOutput = letElement.ToString();

        //    var tokeniser = new Tokeniser(input);
        //    var engine = new CompilationEngine(tokeniser);
        //    XElement result = engine.CompileVarDec();

        //    Assert.AreEqual(expectedOutput, result.ToString());
        //}
    }
}
