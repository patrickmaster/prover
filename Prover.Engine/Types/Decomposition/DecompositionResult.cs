using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    public class DecompositionResult
    {
        public IExpression LeftExpression { get; private set; }

        public IExpression RightExpression { get; private set; }

        public DecompositionType Type { get; private set; }

        public DecompositionResult(DecompositionType type, IExpression expression)
        {
            Type = type;
            LeftExpression = expression;
            RightExpression = null;
        }

        public DecompositionResult(DecompositionType type, IExpression leftExpression, IExpression rightExpression)
        {
            Type = type;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }
    }
}
