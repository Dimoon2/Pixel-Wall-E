using Interpreter.Core.Ast.Expressions;

namespace Interpreter.Core.Ast.Statements
{
    class ColorNode : StatementNode
    {
        public ExpressionNode ColorExpression { get; }

        public ColorNode(ExpressionNode colorExpression)
        {
            ColorExpression = colorExpression ?? throw new ArgumentNullException(nameof(colorExpression));
        }

        public override string ToString()
        {
            return $"Color(Value: {ColorExpression})";
        }
    }
}