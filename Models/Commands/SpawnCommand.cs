using System;
using Avalonia; // Para Point
using PixelWallEApp.Models;
using PixelWallEApp.ViewModels; // Para MainWindowViewModel
using System.Diagnostics; // Para Debug.WriteLine
using PixelWallEApp.Models.Commands;

namespace PixelWallEApp.Models.Commands
{
    public class SpawnCommand //: ICommandDefinition
    {
         public static void Spawn(int x, int y)
        {
        //     if (x >= 0 && x < LogicalCanvasSize && y >= 0 && y < LogicalCanvasSize)
        //     {
        //         _wallE.CurrentPosition = new Point(x, y);
        //         StatusMessage = $"Wall-E spawned at ({x},{y}).";
        //     }
        //     else
        //     {
        //         StatusMessage = $"Spawn Error: Position ({x},{y}) is outside canvas (0-{LogicalCanvasSize-1}). Wall-E not moved.";
        //         Debug.WriteLine(StatusMessage);
        //     }
        // } public void Spawn(int x, int y)
        // {
        //     if (x >= 0 && x < LogicalCanvasSize && y >= 0 && y < LogicalCanvasSize)
        //     {
        //         _wallE.CurrentPosition = new Point(x, y);
        //         StatusMessage = $"Wall-E spawned at ({x},{y}).";
        //     }
        //     else
        //     {
        //         StatusMessage = $"Spawn Error: Position ({x},{y}) is outside canvas (0-{LogicalCanvasSize-1}). Wall-E not moved.";
        //         Debug.WriteLine(StatusMessage);
        //     }
        }
    }
}