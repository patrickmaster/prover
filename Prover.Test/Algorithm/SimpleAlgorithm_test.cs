using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Engine.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Test.Algorithm
{
    [TestClass]
    public class SimpleAlgorithm_test
    {
        private IExpression _falseExpression;

        private IExpression _trueExpression;

        private IAlgorithm _algorithm;

        [TestInitialize]
        public void Init()
        {
            _algorithm = new SimpleAlgorithm();

            _falseExpression = new Negation(
                new Conjunction(
                    new Disjunction(new Literal("p"), new Literal("q")),
                    new Literal("p")));

            _trueExpression = new Implication(
                new Implication(new Literal("p"), new Literal("q")), 
                new Disjunction(new Negation(new Literal("p")), new Literal("q")));
        }

        [TestMethod]
        public void AlgorithmFalseResult()
        {
            AlgorithmResult result = _algorithm.Solve(_falseExpression);

            Assert.AreEqual(4, result.Connections.Count());
            Assert.AreEqual(5, result.Nodes.Count());
        }

        [TestMethod]
        public void AlgorithmTrueResult()
        {
            AlgorithmResult result = _algorithm.Solve(_trueExpression);

            Assert.AreEqual(7, result.Connections.Count());
            Assert.AreEqual(8, result.Nodes.Count());
            Assert.IsTrue(result.IsTautology);
        }
    }
}
