using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types.Expression
{
    public abstract class BinaryOperator : Operator
    {
        public IExpression LeftOperand { get; private set; }
        
        public IExpression RightOperand { get; private set; }

        protected BinaryOperator(IExpression leftOperand, IExpression rightOperand)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }
    }
}
