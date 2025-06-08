using Interpreter.Core.Ast.Statements;

namespace Interpreter.Core.Ast.Expressions
{
    class BinaryOpNode : ExpressionNode
    {
        public ExpressionNode Left { get; }
        public Token Operator { get; }
        public ExpressionNode Right { get; }
        public TokenType OperatorType => Operator.Type;



        public BinaryOpNode(ExpressionNode left, Token operatorToken, ExpressionNode right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Operator = operatorToken ?? throw new ArgumentNullException(nameof(operatorToken));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override string ToString()
        {
            return $"BinaryOp({Left} {Operator.Value} {Right})";
        }
    }
}