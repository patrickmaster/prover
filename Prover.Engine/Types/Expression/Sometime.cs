using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class Sometime : UnaryOperator
    {
        public Sometime(IExpression operand)
            : base(operand)
        {
        }

        protected override DecompositionResult GetDecomposedValues()
        {
            return new DecompositionResult(
                DecompositionType.Delta, 
                Operand);
        }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            return new DecompositionResult(
                DecompositionType.Gamma, 
                new Negation(Operand));
        }

        public override string ToString()
        {
            return "? " + Operand;
        }
    }
}
