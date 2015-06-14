using System;
using Prover.Engine.Types.Decomposition;

namespace Prover.Engine.Types.Expression
{
    public abstract class Operator : IOperator
    {
        private DecompositionResult _decomposed;

        public virtual DecompositionResult Decompose()
        {
            if (IsLiteral)
            {
                throw new InvalidOperationException("Cannot decompose negated literal expression");
            }

            return _decomposed ?? (_decomposed = GetDecomposedValues());
        }

        protected abstract DecompositionResult GetDecomposedValues();

        public virtual bool IsLiteral { get { return false; } }

        public abstract DecompositionResult DecomposeAgainstNegation();
    }
}
