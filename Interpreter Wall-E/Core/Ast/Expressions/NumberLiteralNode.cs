using Interpreter.Core;

namespace Interpreter.Core.Ast.Expressions
{
    public class NumberLiteralNode : ExpressionNode
    {
        public Token NumberToken { get; } // The original NUMBER token from the lexer
        public int Value { get; }         // The integer value of the number

        public NumberLiteralNode(Token numberToken)
        {
            if (numberToken.Type != TokenType.Number || !(numberToken.Literal is int))
            {
                // This is an internal consistency check. The parser should ensure this.
                throw new ArgumentException("NumberLiteralNode requires a NUMBER token with an integer literal.", nameof(numberToken));
            }
            NumberToken = numberToken;
            Value = (int)numberToken.Literal;
        }

        public override string ToString()
        {
            return $"Number({Value})";
        }
    }
}
