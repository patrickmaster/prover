using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class Always : UnaryOperator
    {
        public Always(IExpression operand)
            : base(operand)
        {
        }

        protected override DecompositionResult GetDecomposedValues()
        {
            return new DecompositionResult(
                DecompositionType.Gamma, 
                Operand);
        }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            return new DecompositionResult(
                DecompositionType.Delta,
                new Negation(Operand));
        }

        public override string ToString()
        {
            return "always " + Operand;
        }
    }
}
