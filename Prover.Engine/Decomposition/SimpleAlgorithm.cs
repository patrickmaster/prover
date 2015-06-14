using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Decomposition
{
    public class SimpleAlgorithm : IAlgorithm
    {
        public AlgorithmResult Solve(IExpression rootExpression)
        {
            List<INode> nodes = new List<INode>();
            List<IConnection> connections = new List<IConnection>();
            Stack<INode> toDecompose = new Stack<INode>();

            INode rootNode = new Node(new Negation(rootExpression));
            toDecompose.Push(rootNode);
            nodes.Add(rootNode);

            while (toDecompose.Count > 0)
            {
                INode decomposedNode = toDecompose.Pop();
                IExpression expression = decomposedNode.GetExpression();
                DecompositionResult decompositionResult = expression.Decompose();

                IEnumerable<INode> decomposedNodes;

                if (decompositionResult.Type == DecompositionType.Beta)
                {
                    decomposedNodes = decomposedNode.Branch(decompositionResult.LeftExpression, decompositionResult.RightExpression);

                }
                else
                {
                    decomposedNodes = new List<INode>
                    {
                        decomposedNode.CreateNode(decompositionResult.LeftExpression,
                            decompositionResult.RightExpression)
                    };
                }

                foreach (INode node in decomposedNodes)
                {
                    if (node.CanDecompose)
                    {
                        toDecompose.Push(node);
                    }

                    nodes.Add(node);
                    connections.Add(new Connection(decomposedNode, node));
                }
            }

            return new AlgorithmResult
            {
                Connections = connections,
                Nodes = nodes,
                IsTautology = nodes.Where(x => !x.HasNonLiterals).All(x => x.IsClosed)
            };
        }
    }
}