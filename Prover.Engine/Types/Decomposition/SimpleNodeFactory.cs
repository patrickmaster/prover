using Prover.Engine.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    public class SimpleNodeFactory : INodeFactory
    {
        public INode CreateNode(IExpression expression)
        {
            return new SimpleNode(null, expression);
        }
    }
}
