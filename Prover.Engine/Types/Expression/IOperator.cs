using System.Collections;
using System.Collections.Generic;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public interface IOperator : IExpression
    {
        DecompositionResult DecomposeAgainstNegation();
    }
}
