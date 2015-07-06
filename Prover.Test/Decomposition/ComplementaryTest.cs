using System;
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
    public class ComplementaryTest
    {
        private INodeFactory _nodeFactory;

        [TestInitialize]
        public void Init()
        {
            _nodeFactory = new SimpleNodeFactory();
        }

        [TestMethod]
        public void ShouldBeComplementary()
        {
            INode baseNode = _nodeFactory.CreateNode(new Literal("p"));
            INode node = baseNode.CreateNode(new Literal("q"), new Negation(new Literal("p")));

            Assert.IsTrue(node.IsClosed);
        }

        public void ShouldNotBeComplementary()
        {
            INode baseNode = _nodeFactory.CreateNode(new Literal("p"));
            INode node = baseNode.CreateNode(new Literal("q"), new Literal("p"));

            Assert.IsFalse(node.IsClosed);
        }
    }
}
