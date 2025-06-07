using Interpreter.Core;

namespace Interpreter.Core.Ast.Expressions
{
    public class StringLiteralNode : ExpressionNode
    {
        public Token StringToken { get; } // The original STRING token
        public string Value { get; }      // The actual string content (without quotes)

        public StringLiteralNode(Token stringToken)
        {
            if (stringToken.Type != TokenType.String || !(stringToken.Literal is string))
            {
                throw new ArgumentException("StringLiteralNode requires a STRING token with a string literal.", nameof(stringToken));
            }
            StringToken = stringToken;
            Value = (string)stringToken.Literal;
        }

        public override string ToString()
        {
            // Display the string value with quotes for clarity in AST output
            return $"String(\"{Value}\")";
        }
    }
}

