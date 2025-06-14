// using Avalonia.Media; // For Color
// using System;

// namespace PixelWallEApp.Models.Commands
// {
//     public class DrawLineCommand : ICommandDefinition
//     {
//         public int DirX { get; }
//         public int DirY { get; }
//         public int Distance { get; }

//         public DrawLineCommand(int dirX, int dirY, int distance)
//         {
//             // Basic validation - could be more robust in parser
//             DirX = Math.Clamp(dirX, -1, 1);
//             DirY = Math.Clamp(dirY, -1, 1);
//             Distance = Math.Max(0, distance); // Distance cannot be negative
//         }

//         public string? Execute(WallEState wallEState, CanvasState canvasState)
//         {
          
//         }
//     }
// }