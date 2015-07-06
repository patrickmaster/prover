using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Decomposition;
using QuickGraph;

namespace Prover.UI.Graph
{
    class ProverGraph : BidirectionalGraph<INode, ProverEdge>
    {
        public ProverGraph() : base() { }

        public ProverGraph(bool allowParallelEdges) : base(allowParallelEdges) { }

        public ProverGraph(bool allowParallelEdges, int vertexCapacity) : base(allowParallelEdges, vertexCapacity) { }
    }
}
