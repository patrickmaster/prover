using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types.Decomposition
{
    class Connection : IConnection
    {
        public INode StartNode { get; private set; }
        
        public INode EndNode { get; private set; }

        public Connection(INode startNode, INode endNode)
        {
            StartNode = startNode;
            EndNode = endNode;
        }
    }
}
