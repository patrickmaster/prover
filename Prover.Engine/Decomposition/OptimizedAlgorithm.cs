using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Decomposition
{
    internal class OptimizedAlgorithm : Algorithm
    {
        public override AlgorithmResult Solve(IExpression rootExpression)
        {
            List<INode> nodes = new List<INode>();
            List<IConnection> connections = new List<IConnection>();
            Stack<INode> toDecompose = new Stack<INode>();

            INode rootNode = new OptimizedNode(null, new Negation(rootExpression));
            toDecompose.Push(rootNode);
            nodes.Add(rootNode);

            ((Node)rootNode).IsBranchClosed = true;

            while (toDecompose.Count > 0)
            {
                INode decomposedNode = toDecompose.Pop();
                IExpression expression = decomposedNode.GetExpression();
                DecompositionResult decompositionResult = expression.Decompose();

                IEnumerable<INode> decomposedNodes;

                if (decompositionResult.Type == DecompositionType.Beta)
                {
                    decomposedNodes = decomposedNode.CreateBranch(decompositionResult.LeftExpression, decompositionResult.RightExpression);
                }
                else
                {
                    decomposedNodes = new List<INode>
                    {
                        decomposedNode.CreateNode(decompositionResult.LeftExpression,
                            decompositionResult.RightExpression)
                    };
                }

                connections.AddRange(decomposedNode.Children);

                foreach (INode node in decomposedNodes)
                {
                    nodes.Add(node);

                    ((Node)node).IsBranchClosed = true;
                    
                    if (!node.IsClosed)
                    {
                        if (node.CanDecompose)
                        {
                            toDecompose.Push(node);
                        }
                        else
                        {
                            // this node is still open and cannot be further decomposed,
                            // there is no point in further processing the tree
                            ((Node) node).IsBranchClosed = false;
                            MarkNodeAndParentsOpen((Node) node);

                            return new AlgorithmResult
                            {
                                Connections = connections,
                                Nodes = nodes,
                                IsTautology = false
                            };
                        }
                    }
                }
            }

            return new AlgorithmResult
            {
                Connections = connections,
                Nodes = nodes,
                IsTautology = true
            };
        }
    }
}
