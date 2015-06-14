using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class Implication : BinaryOperator
    {
        public Implication(IExpression leftOperand, IExpression rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
        
        protected override DecompositionResult GetDecomposedValues()
        {
            return new DecompositionResult(
                DecompositionType.Beta, 
                new Negation(LeftOperand),
                RightOperand);
        }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            // alpha rule
            // not (A => B) -> A, not B
            return new DecompositionResult(
                DecompositionType.Alpha,
                LeftOperand,
                new Negation(RightOperand));
        }

        public override string ToString()
        {
            return "( " + LeftOperand + " => " + RightOperand + " )";
        }
    }
}
