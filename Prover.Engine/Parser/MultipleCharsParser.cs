using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Engine.Types;
using Prover.Engine.Types.Expression;

namespace Prover.Engine.Parser
{
    public class MultipleCharsParser : IParser
    {
        private readonly Dictionary<string, OperatorData> _operators = new Dictionary<string, OperatorData>();

        public MultipleCharsParser(OperatorsConfig config)
        {
            _operators.Add(config.Negation.Symbol, new OperatorData(OperatorType.Negation, config.Negation.Priority));
            _operators.Add(config.Always.Symbol, new OperatorData(OperatorType.Always, config.Always.Priority));
            _operators.Add(config.Conjunction.Symbol, new OperatorData(OperatorType.Conjunction, config.Conjunction.Priority));
            _operators.Add(config.Disjunction.Symbol, new OperatorData(OperatorType.Disjunction, config.Disjunction.Priority));
            _operators.Add(config.Equivalence.Symbol, new OperatorData(OperatorType.Equivalence, config.Equivalence.Priority));
            _operators.Add(config.ExclusiveOr.Symbol, new OperatorData(OperatorType.ExclusiveOr, config.ExclusiveOr.Priority));
            _operators.Add(config.Implication.Symbol, new OperatorData(OperatorType.Implication, config.Implication.Priority));
            _operators.Add(config.NegatedConjunction.Symbol, new OperatorData(OperatorType.NegatedConjunction, config.NegatedConjunction.Priority));
            _operators.Add(config.NegatedDisjunction.Symbol, new OperatorData(OperatorType.NegatedDisjunction, config.NegatedDisjunction.Priority));
            _operators.Add(config.Sometime.Symbol, new OperatorData(OperatorType.Sometime, config.Sometime.Priority));
        }

        public IExpression Parse(string expression)
        {
            IEnumerable<Token> rpnCollection = GetRpnCollection(expression);
            Stack<IExpression> expressions = new Stack<IExpression>();

            foreach (Token token in rpnCollection)
            {
                if (token.Type == TokenType.Operator)
                {
                    OperatorData operatorData = _operators[token.Representation];
                    Type operatorType = GetOperatorType(operatorData.Type);
                    int argumentsCount = GetOperatorArgumentsCount(operatorType);
                    IExpression[] arguments = new IExpression[argumentsCount];
                    for (int i = argumentsCount - 1; i >= 0; i--)
                    {
                        // shit happens here
                        arguments[i] = expressions.Pop();
                    }
                    IExpression operatorInstance = (IExpression)Activator.CreateInstance(operatorType, arguments);
                    expressions.Push(operatorInstance);
                }
                else
                {
                    expressions.Push(new Literal(token.Representation));
                }
            }

            IExpression result = expressions.Pop();

            if (expressions.Any())
            {
                throw new ParserException("Invalid input");
            }

            return result;
        }

        private int GetOperatorArgumentsCount(Type operatorType)
        {
            return operatorType.GetConstructors().First().GetParameters().Count();
        }

        private Type GetOperatorType(OperatorType type)
        {
            return Type.GetType("Prover.Engine.Types.Expression." + type.ToString());
        }

        private IEnumerable<Token> GetRpnCollection(string expression)
        {
            IEnumerable<Token> scannedTokens = Scan(expression);
            List<Token> output = new List<Token>();
            Stack<Token> stack = new Stack<Token>();

            foreach (Token token in scannedTokens)
            {
                switch (token.Type)
                {
                    case TokenType.Literal:
                        output.Add(token);
                        break;
                    case TokenType.LeftParen:
                        stack.Push(token);
                        break;
                    case TokenType.RightParen:
                        if (!stack.Any())
                        {
                            throw new ParserException("Too many closing parentheses");
                        }

                        while (stack.Any() && stack.Peek().Type != TokenType.LeftParen)
                        {
                            output.Add(stack.Pop());
                        }

                        // pop the left parenthesis
                        if (stack.Any())
                        {
                            stack.Pop();
                        }
                    
                        break;
                    case TokenType.Operator:
                        Token stackOperator;
                        while (stack.Any() && (stackOperator = stack.Peek()) != null && stackOperator.Type == TokenType.Operator && GetPriority(token) > GetPriority(stackOperator))
                        {
                            output.Add(stack.Pop());
                        }
                        stack.Push(token);
                        break;
                }
            }

            while (stack.Any())
            {
                Token t = stack.Pop();
                if (t.Type == TokenType.Operator)
                {
                    output.Add(t);
                }
                else
                {
                    throw new ParserException("Missing closing parenthesis");
                }
            }

            return output;
        }

        private int GetPriority(Token token)
        {
            if (token.Type == TokenType.Operator)
            {
                return _operators[token.Representation].Priority;
            }

            throw new ParserException(string.Format("Token with representation \"{0}\" is not an operator", token.Representation));
        }

        private IEnumerable<Token> Scan(string expression)
        {
            List<Token> tokens = new List<Token>();
            string op = string.Empty;

            for (int i = 0; i < expression.Count(); i++)
            {
                char c = expression[i];
                char next = i + 1 < expression.Length ? expression[i + 1] : default(char);

                if (char.IsWhiteSpace(c) && op == string.Empty)
                {
                    // skip
                    continue;
                }
                if (char.IsLetter(c) && op == string.Empty)
                {
                    // literal
                    tokens.Add(new Token(new string(new[] { c }), TokenType.Literal));
                }
                else if (c == '(' && op == string.Empty)
                {
                    tokens.Add(new Token("(", TokenType.LeftParen));
                }
                else if (c == ')' && op == string.Empty)
                {
                    tokens.Add(new Token(")", TokenType.RightParen));
                }
                else
                {
                    op += c;

                    if (IsPartOfOperator(op) && (IsPartOfOperator(op + next) || IsOperator(op + next))) continue;
                    if (IsOperator(op))
                    {
                        tokens.Add(new Token(op, TokenType.Operator));
                        op = string.Empty;
                    }
                    else
                    {
                        throw new ParserException(string.Format("Unexpected character \"{0}\"", c),
                            i + 1);
                    }
                }
            }

            return tokens;
        }

        private bool IsPartOfOperator(string op)
        {
            return _operators.Keys.Any(x => x.StartsWith(op) && x != op);
        }

        private bool IsOperator(string op)
        {
            return _operators.ContainsKey(op);
        }

        public IExpression Parse(Stream stream)
        {
            throw new NotImplementedException();
        }
    }

    internal class OperatorData
    {
        public OperatorData(OperatorType type, int priority)
        {
            Type = type;
            Priority = priority;
        }

        public OperatorType Type { get; set; }

        public int Priority { get; set; }
    }

    class Token
    {
        public Token(string representation, TokenType tokenType)
        {
            Representation = representation;
            Type = tokenType;
        }

        public TokenType Type { get; private set; }

        public string Representation { get; private set; }

        public override string ToString()
        {
            return Representation;
        }
    }

    enum TokenType
    {
        Literal,
        Operator,
        LeftParen,
        RightParen
    }
}
