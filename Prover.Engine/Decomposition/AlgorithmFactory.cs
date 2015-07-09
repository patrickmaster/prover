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
                case AlgorithmType.OptimizedContradiction:
                    return new OptimizedContradictionAlgorithm();
                case AlgorithmType.SimpleContradiction:
                    return new SimpleContradictionAlgorithm();
                case AlgorithmType.Optimized:
                    return new OptimizedAlgorithm();
            }

            throw new NotImplementedException("No algorithm for specified type");
        }
    }
}
