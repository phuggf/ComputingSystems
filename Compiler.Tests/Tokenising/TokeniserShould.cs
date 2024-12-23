using Compiler.Tokenising;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Compiler.Tests.Tokenising
{
    [TestClass]
    public class TokeniserShould
    {
        [TestMethod]
        //[Ignore]
        public void ConvertJackFileToTokens()
        {
            var inputFilter = new InputFilter();
            var inputLines = inputFilter.Filter(File.ReadLines("Main.jack"));

            var expectedOutput = File.ReadLines("MainT.xml").ToArray();
            var tokeniser = new Tokeniser(inputLines);

            var tokenElements = new List<XElement>();

            while (tokeniser.MoveNext())
            {
                var tokenType = tokeniser.TokenType.ToString().ToLower();

                switch (tokeniser.TokenType)
                {
                    case TokenType.KEYWORD:
                        tokenElements.Add(new XElement(tokenType, $" {tokeniser.Keyword} "));
                        break;
                    case TokenType.SYMBOL:
                        tokenElements.Add(new XElement(tokenType, $" {tokeniser.Symbol} "));
                        break;
                    case TokenType.IDENTIFIER:
                        tokenElements.Add(new XElement(tokenType, $" {tokeniser.Identifier} "));
                        break;
                    case TokenType.INT_CONST:
                        tokenElements.Add(new XElement("integerConstant", $" {tokeniser.IntValue} "));
                        break;
                    case TokenType.STRING_CONST:
                        tokenElements.Add(new XElement("stringConstant", $" {tokeniser.StringValue} "));
                        break;
                }
            }

            var parentElement = new XElement("tokens");

            foreach ( var tokenElement in tokenElements)
            {
                parentElement.Add(tokenElement);
            }

            var observedOutput = parentElement.ToString().Split(Environment.NewLine).Select(s => s.Trim()).ToArray();


            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], observedOutput[i]);
            }

            Assert.AreEqual(expectedOutput.Length, observedOutput.Length);
        }

        [TestMethod]
        [DataRow([])]
        public void ReturnFalseWhenNoMoreTokens(string[] inputLines)
        {
            var tokeniser = new Tokeniser(inputLines);
            Assert.IsFalse(tokeniser.MoveNext());
        }

        [TestMethod]
        public void ReturnTrueWhenMoreTokens()
        {
            IEnumerable<string> inputLines = ["class Square {"];
            var tokeniser = new Tokeniser(inputLines);
            Assert.IsTrue(tokeniser.MoveNext());
            Assert.IsTrue(tokeniser.MoveNext());
            Assert.IsTrue(tokeniser.MoveNext());
            Assert.IsFalse(tokeniser.MoveNext());
        }

        [TestMethod]
        [DataRow("class", TokenType.KEYWORD)]
        [DataRow("{", TokenType.SYMBOL)]
        [DataRow("32767", TokenType.INT_CONST)]
        [DataRow(@"""test""", TokenType.STRING_CONST)]
        [DataRow("identifier_23", TokenType.IDENTIFIER)]
        public void ReturnTokenType( string input, TokenType expectedTokenType)
        {
            IEnumerable<string> inputLines = [input];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();

            Assert.AreEqual(expectedTokenType, tokeniser.TokenType);
        }

        [TestMethod]
        [DataRow("class")]
        [DataRow("method")]
        [DataRow("true")]
        public void ReturnKeyword(string input)
        {
            IEnumerable<string> inputLines = [input];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();

            Assert.AreEqual(input, tokeniser.Keyword);
        }

        [TestMethod]
        [DataRow(")")]
        [DataRow("<")]
        [DataRow("|")]
        public void ReturnSymbol(string input)
        {
            IEnumerable<string> inputLines = [input];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();

            Assert.AreEqual(input, tokeniser.Symbol);
        }

        [TestMethod]
        [DataRow("test1")]
        [DataRow("another_identifier99")]
        public void ReturnIdentifier(string input)
        {
            IEnumerable<string> inputLines = [input];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();

            Assert.AreEqual(input, tokeniser.Identifier);
        }

        [TestMethod]
        [DataRow("1987", 1987)]
        [DataRow("29446", 29446)]
        public void ReturnIntValue(string input, int expected)
        {
            IEnumerable<string> inputLines = [input];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();

            Assert.AreEqual(expected, tokeniser.IntValue);
        }

        [TestMethod]
        [DataRow(@"""test""", "test")]
        public void ReturnStringValue(string input, string expectedValue)
        {
            IEnumerable<string> inputLines = [input];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();

            Assert.AreEqual(expectedValue, tokeniser.StringValue);
        }

        [TestMethod]
        public void ReturnIdentifierNextToSymbol()
        {
            IEnumerable<string> inputLines = ["test;"];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();
            Assert.AreEqual("test", tokeniser.Identifier);
            tokeniser.MoveNext();
            Assert.AreEqual(";", tokeniser.Symbol);
        }

        [TestMethod]
        public void ReturnAdjacentSymbols()
        {
            IEnumerable<string> inputLines = ["main()"];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();
            Assert.AreEqual("main", tokeniser.Identifier);
            tokeniser.MoveNext();
            Assert.AreEqual("(", tokeniser.Symbol);
            tokeniser.MoveNext();
            Assert.AreEqual(")", tokeniser.Symbol);
        }

        [TestMethod]
        public void ReturnSymbolAfterSpace()
        {
            IEnumerable<string> inputLines = ["main() {"];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();
            tokeniser.MoveNext();
            tokeniser.MoveNext();
            tokeniser.MoveNext();
            Assert.AreEqual("{", tokeniser.Symbol);
        }

        [TestMethod]
        public void ReturnStringConstantWithSpace()
        {
            IEnumerable<string> inputLines = ["s = \"string constant\";"];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();
            tokeniser.MoveNext();
            tokeniser.MoveNext();
            Assert.AreEqual("string constant", tokeniser.StringValue);
        }

        [TestMethod]
        public void ReturnStringConstantThenSymbol()
        {
            IEnumerable<string> inputLines = ["\"string constant\";"];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();
            Assert.AreEqual("string constant", tokeniser.StringValue);
            tokeniser.MoveNext();
            Assert.AreEqual(";", tokeniser.Symbol);
        }

        [TestMethod]
        public void ReturnSymbolThenIdentifier()
        {
            IEnumerable<string> inputLines = ["~exit"];
            var tokeniser = new Tokeniser(inputLines);
            tokeniser.MoveNext();
            Assert.AreEqual("~", tokeniser.Symbol);
            tokeniser.MoveNext();
            Assert.AreEqual("exit", tokeniser.Identifier);
        }
    }
}
