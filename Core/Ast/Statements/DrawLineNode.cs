using System;
using Interpreter.Core.Ast.Expressions;
using Interpreter.Core.Interpreter;
using Avalonia.Media;
using PixelWallEApp.Models;
using Interpreter.Core.Interpreter.Helpers;
namespace Interpreter.Core.Ast.Statements
{
    public class DrawLineNode : StatementNode
    {
        public ExpressionNode ExpresDirX { get; }
        public ExpressionNode ExpresDirY { get; }
        public ExpressionNode ExpresDistance { get; }

        public DrawLineNode(ExpressionNode dirX, ExpressionNode dirY, ExpressionNode distance)
        {
            ExpresDirX = dirX ;
            ExpresDirY = dirY ;
            ExpresDistance = distance;
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
                if (!IsValidDir(DirX) || !IsValidDir(DirY))
                {
                    throw new RuntimeException($"Runtime Error: Drawline directions must be of 1, -1 or 0.");
                }
                if (!interpreter.wallEContext.IsSpawned)
                {
                    throw new RuntimeException($"Runtime Error: Wall-E position not initialized. Use Spawn(x, y) first.");
                }

                if (interpreter.wallEContext.BrushColor == Colors.Transparent)
                {
                    // Moving Wall-E without drawing
                    for (int i = 0; i < Distance; i++)
                    {
                        interpreter.wallEContext.X += DirX;
                        interpreter.wallEContext.Y += DirY;
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

                
                int brushSize = interpreter.wallEContext.BrushSize;
                if (brushSize % 2 == 0)
                {
                    brushSize -= 1; 
                }
                int brushOffset = brushSize / 2; 

                
                for (int i = 0; i <= Distance; i++)
                {
                   
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

                    lastDrawnX = currentX; 
                    lastDrawnY = currentY;
                    currentX += DirX;
                    currentY += DirY;
                }

                interpreter.wallEContext.X = lastDrawnX;
                interpreter.wallEContext.Y = lastDrawnY;
                interpreter.canvas.SetPixel(interpreter.wallEContext.X,interpreter.wallEContext.Y, FunctionHandlers.GetColor("white"));
            }
            else
            {
                throw new RuntimeException($"Drawline parameters must be integers!");
            }
        }

        public static bool IsValidDir(int dir)
        {
            if (dir == 1 || dir == -1 || dir == 0) return true;
            return false;
        }
    }
}