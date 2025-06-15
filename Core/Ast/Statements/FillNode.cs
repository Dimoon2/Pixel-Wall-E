using System;
using System.Drawing;
using Avalonia.Media;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
namespace Interpreter.Core.Ast.Statements
{
    class FillNode : StatementNode
    {
        public FillNode() { }

        public override string ToString()
        {
            return $"Fill()";
        }

        public override void Execute(Interprete interpreter)
        {
            int startX = interpreter.wallEContext.X;
            int startY = interpreter.wallEContext.Y;

            if (startX < 0 || startX >= interpreter.canvas.Size || startY < 0 || startY >= interpreter.canvas.Size)
            {
                throw new RuntimeException("Wall-E is outside the canvas, cannot Fill().");
            }

            Avalonia.Media.Color targetColor = interpreter.canvas.GetPixel(startX, startY);
            Avalonia.Media.Color fillColor = interpreter.wallEContext.BrushColor;

            if (fillColor == targetColor || fillColor == Colors.Transparent)
            {
                interpreter.OutputLog.Add("Fill command resulted in no change.");
                return;
            }

            FloodFillRecursive(startX, startY, interpreter, targetColor, fillColor);

            // --- 5. Notificar a la UI al final ---
            interpreter.canvas.NotifyChanged();
            interpreter.OutputLog.Add($"Fill command executed, starting from ({startX}, {startY}).");
        }

        public void FloodFillRecursive(int x, int y, Interprete interprete, Avalonia.Media.Color targetColor, Avalonia.Media.Color fillColor)
        {
            if (x < 0 || x >= interprete.canvas.Size || y < 0 || y >= interprete.canvas.Size)
            {
                return;
            }

            if (interprete.canvas.GetPixel(x, y) != targetColor)
            {
                return;
            }

            interprete.canvas.SetPixel(x, y, fillColor);

            //recursive call:
            FloodFillRecursive(x + 1, y, interprete, targetColor, fillColor); // Derecha
            FloodFillRecursive(x - 1, y, interprete, targetColor, fillColor); // Izquierda
            FloodFillRecursive(x, y - 1, interprete, targetColor, fillColor); // Arriba
            FloodFillRecursive(x, y + 1, interprete, targetColor, fillColor); // Abajo
        }
    }
}