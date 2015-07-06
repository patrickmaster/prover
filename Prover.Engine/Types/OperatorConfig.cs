using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types
{
    public class OperatorConfig
    {
        public OperatorConfig(OperatorConfig config)
        {
            Symbol = config.Symbol;
            Priority = config.Priority;
        }

        public OperatorConfig()
        {
        }

        public OperatorConfig(string symbol, int priority)
        {
            Symbol = symbol;
            Priority = priority;
        }

        public string Symbol { get; set; }

        public int Priority { get; set; }

    }
}
