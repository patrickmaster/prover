using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types.Expression
{
    public abstract class UnaryOperator : Operator
    {
        public IExpression Operand { get; private set; }

        protected UnaryOperator(IExpression operand)
        {
            Operand = operand;
        }
    }
}
