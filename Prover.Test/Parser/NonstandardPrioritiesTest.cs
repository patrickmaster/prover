using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Engine.Parser;
using Prover.Engine.Types;
using Prover.Engine.Types.Expression;

namespace Prover.Test.Parser
{
    [TestClass]
    public class NonstandardPrioritiesTest
    {
        private OperatorsConfig _operators;
        private IParser _parser;

        [TestInitialize]
        public void Init()
        {
            _operators = new OperatorsConfig
            {
                Negation = new OperatorConfig("~", 3),
                Conjunction = new OperatorConfig("&&", 2),
                Disjunction = new OperatorConfig("||", 1),
                ExclusiveOr = new OperatorConfig("+", 3),
                Implication = new OperatorConfig(">", 1),
                Equivalence = new OperatorConfig("=", 2),
                Always = new OperatorConfig("!", 1),
                Sometime = new OperatorConfig("?", 2),
                NegatedConjunction = new OperatorConfig("%", 2),
                NegatedDisjunction = new OperatorConfig("$", 2)
            };

            _parser = new MultipleCharsParser(_operators);
        }

        [TestMethod]
        public void DisjunctionBeforeConjunction()
        {
            IExpression expression = _parser.Parse("a && b || c");
            Assert.IsInstanceOfType(expression, typeof(Conjunction));

            BinaryOperator op = (BinaryOperator) expression;
            Assert.IsInstanceOfType(op.LeftOperand, typeof(Literal));
            Assert.IsInstanceOfType(op.RightOperand, typeof(Disjunction));
        }
    }
}
