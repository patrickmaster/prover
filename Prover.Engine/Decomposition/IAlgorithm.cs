using Prover.Engine.Types.Expression;

namespace Prover.Engine.Decomposition
{
    public interface IAlgorithm
    {
        AlgorithmResult Solve(IExpression rootExpression);
    }
}