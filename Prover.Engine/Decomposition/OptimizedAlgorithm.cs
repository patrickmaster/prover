﻿using System.Collections.Generic;
using System.Threading;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Decomposition
{
    class OptimizedAlgorithm : Algorithm
    {
        public override AlgorithmResult Solve(IExpression rootExpression)
        {
            throw new System.NotImplementedException();
        }

        public override AlgorithmResult Solve(IExpression rootExpression, CancellationToken cancellationToken)
        {
            return SolveExpression(rootExpression, cancellationToken);
        }

        private AlgorithmResult SolveExpression(IExpression rootExpression, CancellationToken? cancellationToken)
        {
            List<INode> nodes = new List<INode>();
            List<IConnection> connections = new List<IConnection>();
            Stack<INode> toDecompose = new Stack<INode>();

            INode rootNode = new OptimizedNode(null, rootExpression);
            toDecompose.Push(rootNode);
            nodes.Add(rootNode);

            ((Node) rootNode).IsBranchClosed = true;

            while (toDecompose.Count > 0)
            {
                INode decomposedNode = toDecompose.Pop();
                IExpression expression = decomposedNode.GetExpression();
                DecompositionResult decompositionResult = expression.Decompose();

                IEnumerable<INode> decomposedNodes;

                if (decompositionResult.Type == DecompositionType.Beta)
                {
                    decomposedNodes = decomposedNode.CreateBranch(decompositionResult.LeftExpression,
                        decompositionResult.RightExpression);
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

                    ((Node) node).IsBranchClosed = true;

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
                                IsTautology = null,
                                IsTrueable = true
                            };
                        }
                    }
                }

                if (cancellationToken.HasValue)
                {
                    ((CancellationToken) cancellationToken).ThrowIfCancellationRequested();
                }
            }

            return new AlgorithmResult
            {
                Connections = connections,
                Nodes = nodes,
                IsTautology = false,
                IsTrueable = false
            };
        }
    }
}