using Interpreter.Core.Ast.Expressions;

namespace Interpreter.Core.Ast.Statements
{
    class SizeNode : StatementNode
    {
        public ExpressionNode SizeExpression { get; }

        public SizeNode(ExpressionNode numberExpression)
        {
            SizeExpression = numberExpression ?? throw new ArgumentNullException(nameof(numberExpression));
        }

        public override string ToString()
        {
            return $"Size(Value: {SizeExpression})";
        }
    }
}