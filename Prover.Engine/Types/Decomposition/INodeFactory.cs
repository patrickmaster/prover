using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    public interface INodeFactory
    {
        INode CreateNode(IExpression expression);
    }
}