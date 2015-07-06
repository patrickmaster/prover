using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Engine.Types.Decomposition;
using Prover.Engine.Types.Expression;

namespace Prover.Test.Expression
{
    [TestClass]
    public class NegationTest
    {
        [TestMethod]
        public void NegatedDisjunction()
        {
            IExpression negation = new Negation(new Disjunction(new Literal("p"), new Literal("q")));

            DecompositionResult result = negation.Decompose();

            Assert.AreEqual(DecompositionType.Alpha, result.Type);
            Assert.IsInstanceOfType(result.LeftExpression, typeof (Negation));
            Assert.IsInstanceOfType(result.RightExpression, typeof (Negation));
            Assert.IsTrue(result.LeftExpression.IsLiteral);
            Assert.IsTrue(result.RightExpression.IsLiteral);
        }
    }
}
