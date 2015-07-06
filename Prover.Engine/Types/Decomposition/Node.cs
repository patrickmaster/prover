using System;
using System.Collections.Generic;
using System.Linq;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Types.Decomposition
{
    internal abstract class Node : INode
    {
        private int _nonLiteralsCount;
        private bool _isDecomposed;

        protected readonly List<IExpression> _expressions = new List<IExpression>();
        private readonly List<IExpression> _workingExpressions = new List<IExpression>();

        public bool CanDecompose { get { return HasNonLiterals && !_isDecomposed; } }

        public bool HasNonLiterals { get { return _nonLiteralsCount > 0; } }

        protected Node(INode parent, IExpression expression)
        {
            _parent = parent;
            _expressions.Add(expression);
            _workingExpressions.Add(expression);
            if (!expression.IsLiteral)
            {
                _nonLiteralsCount++;
            }
        }

        protected Node(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne)
        {
            _parent = parent;
            _expressions.AddRange(expressions);
            _expressions.Add(expressionOne);
            _workingExpressions.AddRange(expressions);
            _workingExpressions.Add(expressionOne);

            _nonLiteralsCount = _expressions.Count(x => !x.IsLiteral);
        }

        protected Node(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne, IExpression expressionTwo)
        {
            _parent = parent;
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

        public IEnumerable<IExpression> GetAllExpressions()
        {
            return _expressions;
        }

        public INode CreateNode(IExpression expressionOne, IExpression expressionTwo)
        {
            if (_isDecomposed)
            {
                throw new InvalidOperationException("This node has already been decomposed");
            }
            
            if (expressionOne == null) throw new ArgumentNullException("expressionOne");

            INode result = expressionTwo == null ? 
                CreateNode(this, _workingExpressions, expressionOne) : 
                CreateNode(this, _workingExpressions, expressionOne, expressionTwo);

            _isDecomposed = true;
            
            _children.Add(new Connection(this, result));

            return result;
        }

        public IEnumerable<INode> CreateBranch(IExpression expressionOne, IExpression expressionTwo)
        {
            if (expressionOne == null) throw new ArgumentNullException("expressionOne");
            if (expressionTwo == null) throw new ArgumentNullException("expressionTwo");
            if (_isDecomposed)
            {
                throw new InvalidOperationException("This node has already been decomposed");
            }

            _isDecomposed = true;

            List<INode> result = new List<INode>
            {
                CreateNode(this, _workingExpressions, expressionOne),
                CreateNode(this, _workingExpressions, expressionTwo)
            };

            result.ForEach(x => _children.Add(new Connection(this, x)));

            return result;
        }

        readonly List<IConnection> _children = new List<IConnection>(2);

        public IEnumerable<IConnection> Children { get { return _children; } }

        private readonly INode _parent;

        public INode Parent { get { return _parent; } }

        protected static bool CheckIfComplementary(IExpression one, IExpression another)
        {
            string symbolOne = one.ToString();
            string symbolAnother = another.ToString();

            return (symbolOne.Length > symbolAnother.Length && symbolOne == "~ " + symbolAnother
                    || symbolOne.Length < symbolAnother.Length && "~ " + symbolOne == symbolAnother);
        }

        protected abstract Node CreateNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne);

        protected abstract Node CreateNode(INode parent, IEnumerable<IExpression> expressions, IExpression expressionOne, IExpression expressionTwo);

        private bool? _isClosed;

        public bool IsClosed
        {
            get
            {
                if (_isClosed == null)
                {
                    _isClosed = CheckIfClosed();
                }

                return (bool)_isClosed;
            }
        }

        public bool IsBranchClosed { get; set; }

        protected abstract bool CheckIfClosed();

        public override string ToString()
        {
            return string.Join(", ", _expressions);
        }
    }
}