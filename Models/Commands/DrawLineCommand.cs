using Avalonia.Media; // For Color
using System;

namespace PixelWallEApp.Models.Commands
{
    public class DrawLineCommand : ICommandDefinition
    {
        public int DirX { get; }
        public int DirY { get; }
        public int Distance { get; }

        public DrawLineCommand(int dirX, int dirY, int distance)
        {
            // Basic validation - could be more robust in parser
            DirX = Math.Clamp(dirX, -1, 1);
            DirY = Math.Clamp(dirY, -1, 1);
            Distance = Math.Max(0, distance); // Distance cannot be negative
        }

        public string? Execute(WallEState wallEState, CanvasState canvasState)
        {
            if (!wallEState.IsSpawned)
            {
                 return "Runtime Error: Wall-E position not initialized. Use Spawn(x, y) first.";
            }

            if (wallEState.BrushColor == Colors.Transparent)
            {
                // If transparent, just move Wall-E without drawing
                for (int i = 0; i < Distance; i++)
                {
                    wallEState.X += DirX;
                    wallEState.Y += DirY;
                     // Optional: Clamp position if it goes out of bounds during move? Spec doesn't say.
                     // wallEState.X = Math.Clamp(wallEState.X, 0, canvasState.Size - 1);
                     // wallEState.Y = Math.Clamp(wallEState.Y, 0, canvasState.Size - 1);
                }
                 // Final position check (or maybe error if *any* step goes OOB?)
                if (wallEState.X < 0 || wallEState.X >= canvasState.Size || wallEState.Y < 0 || wallEState.Y >= canvasState.Size)
                {
                    // Revert? Or just warn? Let's just set the final position for now as per spec example
                }
                return null;
            }


            int currentX = wallEState.X;
            int currentY = wallEState.Y;
            int lastDrawnX = currentX;
            int lastDrawnY = currentY;

            // Brush size handling (must be odd)
            int brushSize = wallEState.BrushSize;
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
                        if (pixelX >= 0 && pixelX < canvasState.Size && pixelY >= 0 && pixelY < canvasState.Size)
                        {
                            canvasState.SetPixel(pixelX, pixelY, wallEState.BrushColor);
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
            wallEState.X = lastDrawnX;
            wallEState.Y = lastDrawnY;


            // Optional: Add check if final Wall-E position is out of bounds after drawing?
            // The spec example implies the final position is just calculated, even if OOB.
            // The example shows (15,10) after DrawLine(1,0,5) from (10,10).

            return null; // Success
        }
    }
}