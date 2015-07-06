using System.IO;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Parser
{
    public interface IParser
    {
        //IExpression Compute();

        IExpression Parse(string expression);

        IExpression Parse(Stream stream);
    }
}