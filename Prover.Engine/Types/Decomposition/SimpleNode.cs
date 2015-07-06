using System.Collections.Generic;
using System.Linq;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    class SimpleNode : Node
    {

        public SimpleNode(INode parent, IExpression expression) : base(parent, expression)
        {
        }

        private SimpleNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne) : base(parent, expressions, expressionOne)
        {
        }

        private SimpleNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne, IExpression expressionTwo) : base(parent, expressions, expressionOne, expressionTwo)
        {
        }

        protected override Node CreateNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne)
        {
            return new SimpleNode(parent, expressions, expressionOne);
        }

        protected override Node CreateNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne, IExpression expressionTwo)
        {
            return new SimpleNode(parent, expressions, expressionOne, expressionTwo);
        }

        protected override bool CheckIfClosed()
        {
            if (HasNonLiterals)
            {
                return false;
            }

            for (int i = 0; i < _expressions.Count() - 1; i++)
            {
                IExpression one = _expressions[i];
                for (int j = i + 1; j < _expressions.Count(); j++)
                {
                    IExpression another = _expressions[j];

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