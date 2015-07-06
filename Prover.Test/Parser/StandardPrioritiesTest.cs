using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Engine.Parser;
using Prover.Engine.Types;
using Prover.Engine.Types.Expression;

namespace Prover.Test.Parser
{
    [TestClass]
    public class StandardPrioritiesTest
    {
        private OperatorsConfig _operators;
        private IParser _parser;

        [TestInitialize]
        public void Init()
        {
            _operators = new OperatorsConfig
            {
                Negation = new OperatorConfig("~", 1),
                Conjunction = new OperatorConfig("&", 2),
                Disjunction = new OperatorConfig("||", 2),
                ExclusiveOr = new OperatorConfig("+", 2),
                Implication = new OperatorConfig(">", 3),
                Equivalence = new OperatorConfig("=", 3),
                Always = new OperatorConfig("!", 2),
                Sometime = new OperatorConfig("?", 2),
                NegatedConjunction = new OperatorConfig("%", 2),
                NegatedDisjunction = new OperatorConfig("$", 2)
            };

            _parser = new MultipleCharsParser(_operators);
        }

        [TestMethod]
        public void SimpleExpression()
        {
            IExpression expression = _parser.Parse("a ||b");
            Assert.IsInstanceOfType(expression, typeof(Disjunction));

            Disjunction op = (Disjunction)expression;
            Assert.IsInstanceOfType(op.LeftOperand, typeof(Literal));
            Assert.IsInstanceOfType(op.RightOperand, typeof(Literal));
        }

        [TestMethod]
        public void SimpleExpressionWithParentheses()
        {
            IExpression expression = _parser.Parse("(a||b)&b");
            Assert.IsInstanceOfType(expression, typeof(Conjunction));

            Conjunction op = (Conjunction)expression;
            Assert.IsInstanceOfType(op.LeftOperand, typeof(Disjunction));
            Assert.IsInstanceOfType(op.RightOperand, typeof(Literal));
        }

        [TestMethod]
        public void SimpleExpressionWithParenthesesOnRight()
        {
            IExpression expression = _parser.Parse("a||(b&a)");
            Assert.IsInstanceOfType(expression, typeof(Disjunction));

            BinaryOperator op = (BinaryOperator)expression;
            Assert.IsInstanceOfType(op.LeftOperand, typeof(Literal));
            Assert.IsInstanceOfType(op.RightOperand, typeof(Conjunction));
        }

        [TestMethod]
        public void DisjuncitonWithNegationOnRight()
        {
            IExpression expression = _parser.Parse("a||~b");
            Assert.IsInstanceOfType(expression, typeof(Disjunction));

            BinaryOperator op = (BinaryOperator)expression;
            Assert.IsInstanceOfType(op.LeftOperand, typeof(Literal));
            Assert.IsInstanceOfType(op.RightOperand, typeof(Negation));
        }

        [TestMethod]
        public void ConjunctionWithNegationOnLeft()
        {
            IExpression expression = _parser.Parse("~a&b");
            Assert.IsInstanceOfType(expression, typeof(Conjunction));

            BinaryOperator op = (BinaryOperator)expression;
            Assert.IsInstanceOfType(op.LeftOperand, typeof(Negation));
            Assert.IsInstanceOfType(op.RightOperand, typeof(Literal));
        }

        [TestMethod]
        public void ComplexExpression()
        {
            IExpression expression = _parser.Parse("~a & ~ b || (a&~b)");
            Assert.IsInstanceOfType(expression, typeof(Conjunction));

            BinaryOperator op = (BinaryOperator)expression;
            Assert.IsInstanceOfType(op.LeftOperand, typeof(Negation));
            Assert.IsInstanceOfType(op.RightOperand, typeof(Disjunction));

            BinaryOperator rightExpression = (BinaryOperator)op.RightOperand;
            Assert.IsInstanceOfType(rightExpression.LeftOperand, typeof(Negation));
            Assert.IsInstanceOfType(rightExpression.RightOperand, typeof(Conjunction));

            BinaryOperator deepRightExpression = (BinaryOperator)rightExpression.RightOperand;
            Assert.IsInstanceOfType(deepRightExpression.LeftOperand, typeof(Literal));
            Assert.IsInstanceOfType(deepRightExpression.RightOperand, typeof(Negation));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParsingError()
        {
            IExpression expression = _parser.Parse("a&&b");
        }
    }
}
