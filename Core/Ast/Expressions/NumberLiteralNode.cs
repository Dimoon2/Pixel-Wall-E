using System;
using Interpreter.Core;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Expressions
{
    public class NumberLiteralNode : ExpressionNode
    {
        public Token NumberToken { get; }
        public int Value { get; }        

        public NumberLiteralNode(Token numberToken)
        {
            if (numberToken.Type != TokenType.Number || !(numberToken.Literal is int))
            {
               return;
              //  throw new ArgumentException("NumberLiteralNode requires a NUMBER token with an integer literal.", nameof(numberToken));
            }
            NumberToken = numberToken;
            Value = (int)numberToken.Literal;
        }

        public override string ToString()
        {
            return $"Number({Value})";
        }
        public override object Evaluate(Interprete interpreter)
        {
            return Value;
        }
    }
}
