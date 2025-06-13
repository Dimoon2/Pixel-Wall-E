using Interpreter.Core;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Expressions
{
    public class StringLiteralNode : ExpressionNode
    {
        public Token StringToken { get; } 
        public string Value { get; }      

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
            return $"String(\"{Value}\")";
        }
         public override object Evaluate(Interprete interpreter)
        {
            return Value;
        }
    }
}

