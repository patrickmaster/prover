﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Engine.Decomposition;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Test.Decomposition
{
    [TestClass]
    public class NodeTest
    {
        private IExpression _decomposeableAlphaNegation;
        private IExpression _decomposeableComplexAlphaNegation;
        private IExpression _nonDecomposeableNegation;

        private INodeFactory _nodeFactory;

        [TestInitialize]
        public void Init()
        {
            _decomposeableAlphaNegation = new Negation(new Disjunction(new Literal("p"), new Literal("q")));
            
            _nonDecomposeableNegation = new Negation(new Literal("p"));

            _decomposeableComplexAlphaNegation = new Negation(
                new Conjunction(
                    new Disjunction(new Literal("p"), new Literal("q")), 
                    new Literal("p")));

            _nodeFactory = new SimpleNodeFactory();
        }

        [TestMethod]
        public void GetExpressionDecomposable()
        {
            INode node = _nodeFactory.CreateNode(_decomposeableAlphaNegation);

            Assert.IsTrue(node.CanDecompose);
            Assert.IsTrue(node.HasNonLiterals);

            IExpression expression = node.GetExpression();
            Assert.IsInstanceOfType(expression, typeof (Negation));
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void GetExpressionNonDecomposeable()
        {
            INode node = _nodeFactory.CreateNode(_nonDecomposeableNegation);

            Assert.IsFalse(node.CanDecompose);
            Assert.IsFalse(node.HasNonLiterals);

            node.GetExpression();
        }

        [TestMethod]
        public void CreateNode()
        {
            INode node = _nodeFactory.CreateNode(_decomposeableAlphaNegation);
            IExpression expression = node.GetExpression();
            DecompositionResult result = expression.Decompose();

            INode descendantNode = node.CreateNode(result.LeftExpression, result.RightExpression);
            Assert.IsFalse(node.CanDecompose);
            Assert.IsFalse(descendantNode.CanDecompose);
        }

        [TestMethod]
        public void CreateBranch()
        {
            INode node = _nodeFactory.CreateNode(_decomposeableComplexAlphaNegation);
            IExpression expression = node.GetExpression();
            DecompositionResult result = expression.Decompose();

            Assert.AreEqual(DecompositionType.Beta, result.Type);
            IEnumerable<INode> descendants = node.CreateBranch(result.LeftExpression, result.RightExpression);

            Assert.AreEqual(2, descendants.Count());

            INode leftBranch = descendants.First();
            INode rightBranch = descendants.Last();

            Assert.IsTrue(leftBranch.CanDecompose);
            Assert.IsFalse(rightBranch.CanDecompose);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }
    }
}
