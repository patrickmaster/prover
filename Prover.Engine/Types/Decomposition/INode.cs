using System.Collections;
using System.Collections.Generic;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    public interface INode
    {
        IExpression GetExpression();

        IEnumerable<IExpression> GetAllExpressions();

        INode CreateNode(IExpression expressionOne, IExpression expressionTwo);
        
        IEnumerable<INode> CreateBranch(IExpression expressionOne, IExpression expressionTwo);

        IEnumerable<IConnection> Children { get; }

        INode Parent { get; }

        bool CanDecompose { get; }

        bool HasNonLiterals { get; }

        bool IsClosed { get; }

        bool IsBranchClosed { get; }
    }
}