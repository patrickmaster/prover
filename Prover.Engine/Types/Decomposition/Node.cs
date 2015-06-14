using System;
using System.Collections.Generic;
using System.Linq;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    public class Node : INode
    {
        private readonly List<IExpression> _expressions = new List<IExpression>();
        private readonly List<IExpression> _workingExpressions = new List<IExpression>();

        private int _nonLiteralsCount;
        private bool _isDecomposed;

        public Node(IExpression expression)
        {
            _expressions.Add(expression);
            _workingExpressions.Add(expression);
            if (!expression.IsLiteral)
            {
                _nonLiteralsCount++;
            }
        }

        private Node(IEnumerable<IExpression> expressions, IExpression expressionOne)
        {
            _expressions.AddRange(expressions);
            _expressions.Add(expressionOne);
            _workingExpressions.AddRange(expressions);
            _workingExpressions.Add(expressionOne);

            _nonLiteralsCount = _expressions.Count(x => !x.IsLiteral);
        }

        private Node(IEnumerable<IExpression> expressions, IExpression expressionOne, IExpression expressionTwo)
        {
            _expressions.AddRange(expressions);
            _expressions.Add(expressionOne);
            _expressions.Add(expressionTwo);
            _workingExpressions.AddRange(expressions);
            _workingExpressions.Add(expressionOne);
            _workingExpressions.Add(expressionTwo);

            _nonLiteralsCount = _expressions.Count(x => !x.IsLiteral);
        }

        public IExpression GetExpression()
        {
            if (!CanDecompose)
            {
                throw new InvalidOperationException("This node cannot be further decomposed");
            }

            IExpression expression = _workingExpressions.FirstOrDefault(ex => !ex.IsLiteral);
            _workingExpressions.Remove(expression);
            //_nonLiteralsCount--;
            return expression;
        }

        public INode CreateNode(IExpression expressionOne, IExpression expressionTwo)
        {
            if (_isDecomposed)
            {
                throw new InvalidOperationException("This node has already been decomposed");
            }
            
            if (expressionOne == null) throw new ArgumentNullException("expressionOne");

            INode result = expressionTwo == null ? 
                new Node(_workingExpressions, expressionOne) : 
                new Node(_workingExpressions, expressionOne, expressionTwo);

            _isDecomposed = true;
            
            return result;
        }

        public IEnumerable<INode> Branch(IExpression expressionOne, IExpression expressionTwo)
        {
            if (expressionOne == null) throw new ArgumentNullException("expressionOne");
            if (expressionTwo == null) throw new ArgumentNullException("expressionTwo");
            if (_isDecomposed)
            {
                throw new InvalidOperationException("This node has already been decomposed");
            }

            _isDecomposed = true;

            return new List<INode>
            {
                new Node(_workingExpressions, expressionOne), 
                new Node(_workingExpressions, expressionTwo)
            };
        }

        public bool CanDecompose { get { return HasNonLiterals && !_isDecomposed; } }

        public bool HasNonLiterals { get { return _nonLiteralsCount > 0; } }

        public override string ToString()
        {
            return string.Join(", ", _expressions);
        }

        private bool? _isClosed;
        
        public bool IsClosed
        {
            get
            {
                if (_isClosed == null)
                {
                    CheckIfClosed();
                }

                return _isClosed.HasValue && (bool) _isClosed;
            }
        }

        private void CheckIfClosed()
        {
            if (HasNonLiterals)
            {
                return;
            }

            for (int i = 0; i < _expressions.Count() - 1; i++)
            {
                IExpression one = _expressions[i];
                for (int j = i + 1; j < _expressions.Count(); j++)
                {
                    IExpression another = _expressions[j];

                    if (CheckIfComplementary(one, another))
                    {
                        _isClosed = true;
                        return;
                    }
                }
            }
        }

        private bool CheckIfComplementary(IExpression one, IExpression another)
        {
            string symbolOne = one.ToString();
            string symbolAnother = another.ToString();

            return (symbolOne.Length > symbolAnother.Length && symbolOne == "not " + symbolAnother
                || symbolOne.Length < symbolAnother.Length && "not " + symbolOne == symbolAnother);

        }
    }
}