// using System;

// namespace PixelWallEApp.Models.Commands
// {
//     public class SpawnCommand : 
//     ICommandDefinition
//     {
//         public int X { get; }
//         public int Y { get; }

//         public SpawnCommand(int x, int y)
//         {
//             X = x;
//             Y = y;
//         }

//         public string? Execute(WallEState wallEState, CanvasState canvasState)
//         {
//             // Spec: Only one Spawn allowed, must be the first command (we'll enforce this in Interpreter)
//             // Spec: Runtime error if position is outside bounds
//             if (X < 0 || X >= canvasState.Size || Y < 0 || Y >= canvasState.Size)
//             {
//                 return $"Runtime Error: Spawn position ({X}, {Y}) is outside the canvas bounds (0-{canvasState.Size - 1}).";
//             }

//             wallEState.X = X;
//             wallEState.Y = Y;
//             wallEState.IsSpawned = true;
//             return null; // Success
//         }
//     }
// }