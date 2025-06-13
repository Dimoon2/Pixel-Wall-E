using Interpreter.Core.Ast.Statements;
using Interpreter.Core.Interpreter;
using Interpreter.Core.Interpreter.Helpers;
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

        public override object Evaluate(Interprete interpreter)
        {
            object leftValue = Left.Evaluate(interpreter);

            // Handle && and || here
            if (Operator.Type == TokenType.And)
            {
                if (!BinaryOperations.ConvertToBooleanStatic(leftValue)) return false;
                object rightValueAnd = Right.Evaluate(interpreter);
                return BinaryOperations.ConvertToBooleanStatic(rightValueAnd);
            }
            if (Operator.Type == TokenType.Or)
            {
                if (BinaryOperations.ConvertToBooleanStatic(leftValue)) return true;
                object rightValueOr = Right.Evaluate(interpreter);
                return BinaryOperations.ConvertToBooleanStatic(rightValueOr);
            }

            // For other operators, evaluate right operand and use the handler
            object rightValue = Right.Evaluate(interpreter);

            if (BinaryOperations.TryGetHandler(Operator.Type, out BinaryOperationHandler handler))
            {
                try
                {
                    return handler(leftValue, rightValue);
                }
                catch (RuntimeException) { throw; } // Re-throw exceptions
                catch (Exception) // Catch other unexpected errors from handlers
                {
                    throw; // new RuntimeException($"Error during binary operation '{node.Operator.Value}': {ex.Message}", ex);
                }
            }

            throw new RuntimeException($"Unsupported binary operator: {Operator.Type}");
        }
    }
}