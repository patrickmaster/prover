using System.Collections.Generic;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Decomposition
{
    public class AlgorithmResult
    {
        public bool IsTautology { get; set; }

        public IEnumerable<INode> Nodes { get; set; }

        public IEnumerable<IConnection> Connections { get; set; } 
    }
}
