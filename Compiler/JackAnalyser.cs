
using Compiler.Compilation;
using Compiler.Tokenising;
using Core;

namespace Compiler
{
    public class JackAnalyser
    {
        public JackAnalyser()
        {
        }

        public IEnumerable<string> Compile(IEnumerable<string> inputLines)
        {
            var inputFilter = new InputFilter();
            var tokeniser = new Tokeniser(inputFilter.Filter(inputLines));
            var engine = new CompilationEngine(tokeniser);
            var xml = engine.CompileClass().ToString();
            return xml.Split(Environment.NewLine);
        }
    }
}