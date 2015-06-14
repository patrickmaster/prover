using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types.Expression
{
    public abstract class UnaryOperator : Operator
    {
        protected readonly IExpression Operand;

        protected UnaryOperator(IExpression operand)
        {
            Operand = operand;
        }
    }
}
