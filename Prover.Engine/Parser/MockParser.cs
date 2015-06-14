using System.IO;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Parser
{
    public class MockParser : IParser
    {
        public IExpression Parse(string expression)
        {
            return new Implication(
                new Implication(new Literal("p"), new Literal("q")),
                new Disjunction(new Negation(new Literal("p")), new Literal("q")));
        }

        public IExpression Parse(Stream stream)
        {
            return new Implication(
                new Implication(new Conjunction( new Literal("p"), new Negation(new Literal("r"))), new Literal("q")),
                new Disjunction(new Negation(new Literal("p")), new Literal("q")));
        }
    }
}