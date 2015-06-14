using System.Collections.Generic;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    public interface INode
    {
        IExpression GetExpression();

        INode CreateNode(IExpression expressionOne, IExpression expressionTwo);
        
        IEnumerable<INode> Branch(IExpression expressionOne, IExpression expressionTwo);

        bool CanDecompose { get; }

        bool HasNonLiterals { get; }

        bool IsClosed { get; }
    }
}