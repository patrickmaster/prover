using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;
using QuickGraph;

namespace Prover.UI.Graph
{
    class ProverEdge : Edge<INode>
    {
        public ProverEdge(INode startNode, INode endNode) : base(startNode, endNode)
        {
        }
    }
}
