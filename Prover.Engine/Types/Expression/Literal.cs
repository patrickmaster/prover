using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class Literal : IExpression
    {
        private readonly string _symbol;

        public Literal(string symbol)
        {
            _symbol = symbol;
        }

        public DecompositionResult Decompose()
        {
            throw new InvalidOperationException("Cannot decompose literal expression");
        }

        public bool IsLiteral
        {
            get { return true; }
        }

        public override string ToString()
        {
            return _symbol;
        }
    }
}
