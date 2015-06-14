using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class NegatedConjunction : BinaryOperator
    {
        public NegatedConjunction(IExpression leftOperand, IExpression rightOperand)
            : base(leftOperand, rightOperand)
        {
        }

        protected override DecompositionResult GetDecomposedValues()
        {
            return new DecompositionResult(
                DecompositionType.Beta, 
                new Negation(LeftOperand),
                new Negation(RightOperand));
        }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            // alpha rule
            // not (A nand B) -> A, B
            return new DecompositionResult(
                DecompositionType.Alpha,
                LeftOperand,
                RightOperand);
        }

        public override string ToString()
        {
            return "( " + LeftOperand + " nand " + RightOperand + " )";
        }
    }
}
