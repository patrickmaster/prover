using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class Equivalence : BinaryOperator
    {
        public Equivalence(IExpression leftOperand, IExpression rightOperand)
            : base(leftOperand, rightOperand)
        {
        }

        protected override DecompositionResult GetDecomposedValues()
        {
            return new DecompositionResult(
                DecompositionType.Alpha, 
                new Implication(LeftOperand, RightOperand),
                new Implication(RightOperand, LeftOperand));
        }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            // beta rule
            // not (A <=> B) -> not (A => B), not (B => A)
            return new DecompositionResult(
                DecompositionType.Beta,
                new Negation(new Implication(LeftOperand, RightOperand)),
                new Negation(new Implication(RightOperand, LeftOperand)));
        }

        public override string ToString()
        {
            return "( " + LeftOperand + " <=> " + RightOperand + " )";
        }
    }
}
