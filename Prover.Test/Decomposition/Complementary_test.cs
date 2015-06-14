using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Test.Decomposition
{
    [TestClass]
    public class Complementary_test
    {
        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void ShouldBeComplementary()
        {
            INode baseNode = new Node(new Literal("p"));
            INode node = baseNode.CreateNode(new Literal("q"), new Negation(new Literal("p")));

            Assert.IsTrue(node.IsClosed);
        }

        public void ShouldNotBeComplementary()
        {
            INode baseNode = new Node(new Literal("p"));
            INode node = baseNode.CreateNode(new Literal("q"), new Literal("p"));

            Assert.IsFalse(node.IsClosed);
        }
    }
}
