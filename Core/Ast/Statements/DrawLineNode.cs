using System;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
using Avalonia.Media;
namespace Interpreter.Core.Ast.Statements
{
    class DrawLineNode : StatementNode
    {
        public ExpressionNode ExpresDirX { get; }
        public ExpressionNode ExpresDirY { get; }
        public ExpressionNode ExpresDistance { get; }

        public DrawLineNode(ExpressionNode dirX, ExpressionNode dirY, ExpressionNode distance)
        {
            ExpresDirX = dirX ?? throw new ArgumentNullException(nameof(dirX));
            ExpresDirY = dirY ?? throw new ArgumentNullException(nameof(dirY));
            ExpresDistance = distance ?? throw new ArgumentNullException(nameof(distance));
        }

        public override string ToString()
        {
            return $"DrawLine(DirX: {ExpresDirX}, DirY: {ExpresDirY}, Distance: {ExpresDistance})";
        }

        public override void Execute(Interprete interpreter)
        {
            object dirX = ExpresDirX.Evaluate(interpreter);
            object dirY = ExpresDirY.Evaluate(interpreter);
            object dist = ExpresDistance.Evaluate(interpreter);
            int Size = interpreter.canvas.Size;
            if (dirX is int DirX && dirY is int DirY && dist is int Distance)
            {
                if (!interpreter.wallEContext.IsSpawned)
                {
                    throw new RuntimeException($"Runtime Error: Wall-E position not initialized. Use Spawn(x, y) first.");

                }

                if (interpreter.wallEContext.BrushColor == Colors.Transparent)
                {
                    // If transparent, just move Wall-E without drawing
                    for (int i = 0; i < Distance; i++)
                    {
                        interpreter.wallEContext.X += DirX;
                        interpreter.wallEContext.Y += DirY;
                        // Optional: Clamp position if it goes out of bounds during move? Spec doesn't say.
                        // wallEState.X = Math.Clamp(wallEState.X, 0, canvasState.Size - 1);
                        // wallEState.Y = Math.Clamp(wallEState.Y, 0, canvasState.Size - 1);
                    }
                    // Final position check (or maybe error if *any* step goes OOB?)
                    if (interpreter.wallEContext.X < 0 || interpreter.wallEContext.X >= Size || interpreter.wallEContext.Y < 0 || interpreter.wallEContext.Y >= Size)
                    {
                        throw new RuntimeException($"Runtime Error: Wall-E position is out of the canvas bounds!");

                    }
                }
                int currentX = interpreter.wallEContext.X;
                int currentY = interpreter.wallEContext.Y;
                int lastDrawnX = currentX;
                int lastDrawnY = currentY;

                // Brush size handling (must be odd)
                int brushSize = interpreter.wallEContext.BrushSize;
                if (brushSize % 2 == 0)
                {
                    brushSize = Math.Max(1, brushSize - 1); // Use odd size immediately smaller
                }
                int brushOffset = brushSize / 2; // Integer division gives offset from center

                // Draw points along the line
                for (int i = 0; i < Distance; i++)
                {
                    // Apply brush centered at (currentX, currentY)
                    for (int brushY = -brushOffset; brushY <= brushOffset; brushY++)
                    {
                        for (int brushX = -brushOffset; brushX <= brushOffset; brushX++)
                        {
                            int pixelX = currentX + brushX;
                            int pixelY = currentY + brushY;

                            // Check bounds before drawing
                            if (pixelX >= 0 && pixelX < interpreter.canvas.Size && pixelY >= 0 && pixelY < Size)
                            {
                                interpreter.canvas.SetPixel(pixelX, pixelY, interpreter.wallEContext.BrushColor); 

                                
                            }
                        }
                    }

                    // Update position for the *next* point (or final position)
                    lastDrawnX = currentX; // Remember the center of the last drawn brush square
                    lastDrawnY = currentY;
                    currentX += DirX;
                    currentY += DirY;
                }

                // Spec: "new position of Wall-E will be en the end of the line (last pixel drawn)"
                // This means the center of the last drawn brush square.
                interpreter.wallEContext.X = lastDrawnX;
                interpreter.wallEContext.Y = lastDrawnY;


                // Optional: Add check if final Wall-E position is out of bounds after drawing?
                // The spec example implies the final position is just calculated, even if OOB.
                // The example shows (15,10) after DrawLine(1,0,5) from (10,10).
            }
            else
            {
                throw new RuntimeException($"Drawline parameters must be integers!");
            }
        }
    }
}