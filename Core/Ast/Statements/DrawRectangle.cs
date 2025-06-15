using System;
using Avalonia.Media;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
using Interpreter.Core.Interpreter.Helpers;
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
            object exDir = ExpresDirX.Evaluate(interpreter);
            object eyDir = ExpresDirY.Evaluate(interpreter);
            object dist = ExpresDistance.Evaluate(interpreter);
            object wid = Width.Evaluate(interpreter);
            object hei = Height.Evaluate(interpreter);

            if (exDir is not int dx || eyDir is not int dy || dist is not int d || wid is not int width || hei is not int height)
            {
                throw new RuntimeException($"Runtime Error: arguments must be integers");
            }

            if (!DrawLineNode.IsValidDir(dx) || !DrawLineNode.IsValidDir(dy))
            {
                throw new RuntimeException($"Runtime Error: DrawRectangle directions must be of 1, -1 or 0.");
            }
            if (!isPositive(d) || !isPositive(width) || !isPositive(height))
            {
                throw new RuntimeException($"Runtime Error: DrawRectangle direction, width and height must be positive!.");
            }
            int cx = interpreter.wallEContext.X;
            int cy = interpreter.wallEContext.Y;

            cx = cx + (dx * d);
            cy = cy + (dy * d);

            if (interpreter.wallEContext.BrushColor == FunctionHandlers.GetColor("Transparent"))
            {
                interpreter.wallEContext.X += dx * d;
                interpreter.wallEContext.Y += dy * d;
                return;
            }

            // --- 2. Calcular la Posición del Centro ---
            int centerX = interpreter.wallEContext.X + (dx * d);
            int centerY = interpreter.wallEContext.Y + (dy * d);

            // --- 3. Calcular las Coordenadas del Rectángulo ---
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            int startX = centerX - halfWidth;
            int startY = centerY - halfHeight;
            int endX = centerX + halfWidth;
            int endY = centerY + halfHeight;

            int brushSize = interpreter.wallEContext.BrushSize;
            int brushOffset = brushSize / 2;
            Color brushColor = interpreter.wallEContext.BrushColor;

            for (int x = startX; x <= endX; x++)
            {
                DrawPixelWithBrush(x, startY, interpreter, brushOffset, brushColor);
            }

            // Línea inferior (horizontal)
            for (int x = startX; x <= endX; x++)
            {
                DrawPixelWithBrush(x, endY, interpreter, brushOffset, brushColor);
            }

            // Línea izquierda (vertical)
            // Empezamos en startY + 1 y terminamos en endY - 1 para no redibujar las esquinas.
            for (int y = startY + 1; y < endY; y++)
            {
                DrawPixelWithBrush(startX, y, interpreter, brushOffset, brushColor);
            }

            // Línea derecha (vertical)
            for (int y = startY + 1; y < endY; y++)
            {
                DrawPixelWithBrush(endX, y, interpreter, brushOffset, brushColor);
            }

            interpreter.wallEContext.X = centerX;
            interpreter.wallEContext.Y = centerY;

            interpreter.canvas.NotifyChanged();
        }

        public bool isPositive(int k)
        {
            if (k > 0) return true;
            return false;
        }

        public void DrawPixelWithBrush(int px, int py, Interprete interprete, int brushOffset, Color brushColor)
        {
            for (int dy = -brushOffset; dy <= brushOffset; dy++)
            {
                for (int dx = -brushOffset; dx <= brushOffset; dx++)
                {
                    interprete.canvas.SetPixel(px + dx, py + dy, brushColor);
                }
            }
        }
    }
}
