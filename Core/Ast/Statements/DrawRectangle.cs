using System;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    class DrawRectangleNode : StatementNode
    {
        public ExpressionNode ExpresDirX { get; }
        public ExpressionNode ExpresDirY { get; }
        public ExpressionNode ExpresDistance { get; }
        public ExpressionNode Width { get; }
        public ExpressionNode Height { get; }

        //(int dirX, int dirY, int distance, int width, int height
        public DrawRectangleNode(ExpressionNode expres1, ExpressionNode expres2, ExpressionNode expres3, ExpressionNode width, ExpressionNode height)
        {
            ExpresDirX = expres1 ?? throw new ArgumentNullException(nameof(expres1));
            ExpresDirY = expres2 ?? throw new ArgumentNullException(nameof(expres2));
            ExpresDistance = expres3 ?? throw new ArgumentNullException(nameof(expres3));
            Width = width ?? throw new ArgumentNullException(nameof(width));
            Height = height ?? throw new ArgumentNullException(nameof(height));
        }

        public override string ToString()
        {
            return $"DrawRectangleNode(DirX: {ExpresDirX}, DirY: {ExpresDirY}, Distance: {ExpresDistance}, Wigth: {Width}, Height: {Height})";
        }

        public override void Execute(Interprete interpreter)
        {
            throw new NotImplementedException();
        }
    }
}