using Interpreter.Core.Ast.Expressions;

namespace Interpreter.Core.Ast.Statements
{
    class DrawCircleNode : StatementNode
    {
        public ExpressionNode ExpresDirX { get; }
        public ExpressionNode ExpresDirY { get; }
        public ExpressionNode Radius { get; }

        public DrawCircleNode(ExpressionNode expres1, ExpressionNode expres2, ExpressionNode expres3)
        {
            ExpresDirX= expres1 ?? throw new ArgumentNullException(nameof(expres1));
            ExpresDirY= expres2 ?? throw new ArgumentNullException(nameof(expres2));
            Radius = expres3 ?? throw new ArgumentNullException(nameof(expres3));
        }

        public override string ToString()
        {
            return $"DrawCircle(DirX: {ExpresDirX}, DirY: {ExpresDirY}, Radius: {Radius})";
        }
    }
}