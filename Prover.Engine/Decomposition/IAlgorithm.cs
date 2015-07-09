using System.Threading;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Decomposition
{
    public interface IAlgorithm
    {
        AlgorithmResult Solve(IExpression rootExpression);
        
        AlgorithmResult Solve(IExpression rootExpression, CancellationToken cancellationToken);
    }
}