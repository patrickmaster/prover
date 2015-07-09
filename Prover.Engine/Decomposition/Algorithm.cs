using System.Threading;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Decomposition
{
    internal abstract class Algorithm : IAlgorithm
    {
        protected void MarkNodeAndParentsOpen(Node node)
        {
            Node parent;
            node.IsBranchClosed = false;

            while ((parent = (Node) node.Parent) != null)
            {
                parent.IsBranchClosed = false;
                node = parent;
            }
        }

        public abstract AlgorithmResult Solve(IExpression rootExpression);
        
        public abstract AlgorithmResult Solve(IExpression rootExpression, CancellationToken cancellationToken);
    }
}