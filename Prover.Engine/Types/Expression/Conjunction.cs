using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class Conjunction : BinaryOperator
    {
        public Conjunction(IExpression leftOperand, IExpression rightOperand)
            : base(leftOperand, rightOperand)
        {
        }

        protected override DecompositionResult GetDecomposedValues()
        {
            return new DecompositionResult(
                DecompositionType.Alpha, 
                LeftOperand,
                RightOperand);
        }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            // beta rule
            // not (A and B) -> not A, not B
            return new DecompositionResult(
                DecompositionType.Beta,
                new Negation(LeftOperand),
                new Negation(RightOperand));
        }

        public override string ToString()
        {
            return "( " + LeftOperand + " and " + RightOperand + " )";
        }
    }
}
