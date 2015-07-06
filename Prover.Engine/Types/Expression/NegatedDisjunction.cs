using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class NegatedDisjunction : BinaryOperator
    {
        public NegatedDisjunction(IExpression leftOperand, IExpression rightOperand)
            : base(leftOperand, rightOperand)
        {
        }

        protected override DecompositionResult GetDecomposedValues()
        {
            return new DecompositionResult(
                DecompositionType.Alpha, 
                new Negation(LeftOperand),
                new Negation(RightOperand));
        }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            // beta rule
            // not (A nor B) -> A, B
            return new DecompositionResult(
                DecompositionType.Beta, 
                LeftOperand,
                RightOperand);
        }

        public override string ToString()
        {
            return "( " + LeftOperand + " ~| " + RightOperand + " )";
        }
    }
}
