using System;
using Interpreter.Core.Ast.Statements;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Expressions
{
    class UnaryOpNode : ExpressionNode
    {
        public Token Operator { get; }
        public ExpressionNode Value { get; }
        public TokenType OperatorType => Operator.Type;



        public UnaryOpNode(Token operatorToken, ExpressionNode value)
        {
            Operator = operatorToken ?? throw new ArgumentNullException(nameof(operatorToken));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString()
        {
            return $"UnaryOp {Operator.Value} {Value})";
        }
        public override object Evaluate(Interprete interpreter)
        {
            object Right = Value.Evaluate(interpreter);
            if (Right is int rVal)
            {
                return -rVal;
            }
            throw new RuntimeException($"Expected value of type int");
        }
    }
}