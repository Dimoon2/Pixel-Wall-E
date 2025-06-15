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
            object exDir = ExpresDirX.Evaluate(interpreter);
            object eyDir = ExpresDirY.Evaluate(interpreter);
            object radius = Radius.Evaluate(interpreter);

            if (exDir is not int dx || eyDir is not int dy || radius is not int r)
            {
                throw new RuntimeException($"Runtime Error: arguments must be integers");
            }

            if (!DrawLineNode.IsValidDir(dx) || !DrawLineNode.IsValidDir(dy))
            {
                throw new RuntimeException($"Runtime Error: DrawCircle directions must be of 1, -1 or 0.");
            }
            if(r < 0)
            {
                throw new RuntimeException($"Runtime Error: DrawCircle radius must be positive!.");
            }
            int cx = interpreter.wallEContext.X;
            int cy = interpreter.wallEContext.Y;

            cx = cx + (dx * r);
            cy = cy + (dy * r);

            int x = 0;
            int y = -r;
            int p = -r;

            while (x < -y)
            {
                if (p > 0)
                {
                    y++;
                    p += 2 * (x + y) + 1;
                }
                else
                { p += 2 * x + 1; }

                interpreter.canvas.SetPixel(cx + x, cy + y, interpreter.wallEContext.BrushColor);
                interpreter.canvas.SetPixel(cx - x, cy + y, interpreter.wallEContext.BrushColor);
                interpreter.canvas.SetPixel(cx + x, cy - y, interpreter.wallEContext.BrushColor);
                interpreter.canvas.SetPixel(cx - x, cy - y, interpreter.wallEContext.BrushColor);
                interpreter.canvas.SetPixel(cx + y, cy + x, interpreter.wallEContext.BrushColor);
                interpreter.canvas.SetPixel(cx + y, cy - x, interpreter.wallEContext.BrushColor);
                interpreter.canvas.SetPixel(cx - y, cy + x, interpreter.wallEContext.BrushColor);
                interpreter.canvas.SetPixel(cx - y, cy - x, interpreter.wallEContext.BrushColor);

                x++;
            }
            interpreter.wallEContext.X = cx;
            interpreter.wallEContext.Y = cy;
        }
    }
}