using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Engine.Types
{
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {

        }

        public ParserException(string message, int i)
            : base(message)
        {
            CharacterNumber = i;
        }

        public int CharacterNumber { get; private set; }
    }
}
