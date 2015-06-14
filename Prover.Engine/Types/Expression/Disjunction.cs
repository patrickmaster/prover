using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class Disjunction : BinaryOperator
    {
        public Disjunction(IExpression leftExpression, IExpression rightExpression)
            : base(leftExpression, rightExpression)
        {
        }

        protected override DecompositionResult GetDecomposedValues()
        {
            return new DecompositionResult(
                DecompositionType.Beta, 
                LeftOperand, 
                RightOperand);
        }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            // alpha rule
            // not (A or B) -> not A, not B
            return new DecompositionResult(
                DecompositionType.Alpha,
                new Negation(LeftOperand),
                new Negation(RightOperand));
        }

        public override string ToString()
        {
            return "( " + LeftOperand + " or " + RightOperand + " )";
        }
    }
}
