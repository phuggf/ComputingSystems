using Compiler.Tokenising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Compilation
{
    public class CompilationEngine(Tokeniser tokeniser)
    {
        private Tokeniser _tokeniser = tokeniser;
    }
}
