using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    class OptimizedNode : Node
    {
        public OptimizedNode(INode parent, IExpression expression) : base(parent, expression)
        {
        }

        public OptimizedNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne) : base(parent, expressions, expressionOne)
        {
        }

        public OptimizedNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne, IExpression expressionTwo) : base(parent, expressions, expressionOne, expressionTwo)
        {
        }

        protected override Node CreateNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne)
        {
            return new OptimizedNode(parent, expressions, expressionOne);
        }

        protected override Node CreateNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne, IExpression expressionTwo)
        {
            return new OptimizedNode(parent, expressions, expressionOne, expressionTwo);
        }

        protected override bool CheckIfClosed()
        {
            IExpression[] literals = _expressions.Where(x => x.IsLiteral).ToArray();

            for (int i = 0; i < literals.Count() - 1; i++)
            {
                IExpression one = literals[i];
                for (int j = i + 1; j < literals.Count(); j++)
                {
                    IExpression another = literals[j];

                    if (CheckIfComplementary(one, another))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
