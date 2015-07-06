using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Decomposition
{
    public class AlgorithmFactory
    {
        public static IAlgorithm GetAlgorithm(AlgorithmType type)
        {
            switch (type)
            {
                case AlgorithmType.Optimized:
                    return new OptimizedAlgorithm();
                    case AlgorithmType.Simple:
                    return new SimpleAlgorithm();
            }

            throw new NotImplementedException("No algorithm for specified type");
        }
    }
}
