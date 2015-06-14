using System;
using System.Collections.Generic;
using System.Linq;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public class Negation : UnaryOperator
    {
        private readonly bool _isLiteral;

        public Negation(IExpression expression) : base(expression)
        {
            _isLiteral = expression is Literal;
        }

        protected override DecompositionResult GetDecomposedValues()
        {
            // the operand cannot be a literal expression
            // if it was, then this method wouldn't get called
            IOperator operand = Operand as IOperator;

            return operand.DecomposeAgainstNegation();
        }

        public override bool IsLiteral { get { return _isLiteral; } }

        public override DecompositionResult DecomposeAgainstNegation()
        {
            // alpha rule
            // not (not A) -> A
            return new DecompositionResult(DecompositionType.Alpha, Operand);
        }

        public override string ToString()
        {
            return "not " + Operand;
        }
    }
}
