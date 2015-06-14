using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types.Decomposition
{
    public interface IConnection
    {
        INode StartNode { get; }

        INode EndNode { get; }
    }
}
