using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types.Expression
{
    public abstract class BinaryOperator : Operator
    {
        protected readonly IExpression LeftOperand;
        protected readonly IExpression RightOperand;

        protected BinaryOperator(IExpression leftOperand, IExpression rightOperand)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }
    }
}
