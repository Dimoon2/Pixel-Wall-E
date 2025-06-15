using System;
using Avalonia.Controls.Platform;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    class DrawCircleNode : StatementNode
    {
        public ExpressionNode ExpresDirX { get; }
        public ExpressionNode ExpresDirY { get; }
        public ExpressionNode Radius { get; }

        public DrawCircleNode(ExpressionNode expres1, ExpressionNode expres2, ExpressionNode expres3)
        {
            ExpresDirX = expres1 ?? throw new ArgumentNullException(nameof(expres1));
            ExpresDirY = expres2 ?? throw new ArgumentNullException(nameof(expres2));
            Radius = expres3 ?? throw new ArgumentNullException(nameof(expres3));
        }

        public override string ToString()
        {
            return $"DrawCircle(DirX: {ExpresDirX}, DirY: {ExpresDirY}, Radius: {Radius})";
        }

        public override void Execute(Interprete interpreter)
        {
            // int cx = (int)ExpresDirX.Evaluate(interpreter);
            // int cy = (int)ExpresDirY.Evaluate(interpreter);

            // int x = 0;
            // int y = -(int)Radius.Evaluate(interpreter);
            // while (x < -y)
            // {
            //     x++;
            //     interpreter.canvas.SetPixel(cx+x, cy+y);
            // }
            throw new NotImplementedException();
        }
    }
}