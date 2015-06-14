using System.Collections.Generic;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public interface IExpression
    {
        DecompositionResult Decompose();

        bool IsLiteral { get; }
    }
}