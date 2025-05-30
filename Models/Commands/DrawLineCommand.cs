// Models/Commands/DrawLineCommand.cs
using Avalonia; // Para Point
using System.Collections.ObjectModel;
using PixelWallEApp.Models;
using System;
using System.Diagnostics; // Para Debug.WriteLine si aún se usa
using PixelWallEApp.Models.Commands;
using Avalonia.Media;

namespace PixelWallEApp.Models.Commands
{
    public class DrawLineCommand : ICommandDefinition
    {
        private readonly int _dirX;
        private readonly int _dirY;
        private readonly int _distance;

        public DrawLineCommand(int dirX, int dirY, int distance)
        {
            _dirX = dirX;
            _dirY = dirY;
            _distance = distance;
        }

        private bool IsDirectionValid(int dirValue)
        {
            return dirValue >= -1 && dirValue <= 1;
        }

        public bool Execute(
            WallEStateModel wallEState,
            ObservableCollection<LineModel> linesToDraw,
            int logicalCanvasSize,
            Action<string> statusReporter)
        {
            if (!IsDirectionValid(_dirX) || !IsDirectionValid(_dirY) || _distance <= 0)
            {
                statusReporter($"DrawLineCommand Error: Invalid parameters provided to command (dirX:{_dirX}, dirY:{_dirY}, dist:{_distance}).");
                Debug.WriteLine($"DrawLineCommand Error: Invalid parameters (dirX:{_dirX}, dirY:{_dirY}, dist:{_distance}).");
                statusReporter("\nThe build failed. Fix the build errors and run again.");//ponerlo rojo
                return false; // Fallo en los parámetros del comando
            }

            Point startPoint = wallEState.CurrentPosition;
            Point tentativeEndPoint = new Point(
                startPoint.X + (_dirX * _distance),
                startPoint.Y + (_dirY * _distance)
            );

            int finalX = (int)tentativeEndPoint.X;
            int finalY = (int)tentativeEndPoint.Y;

            if (finalX < 0 || finalX >= logicalCanvasSize || finalY < 0 || finalY >= logicalCanvasSize)
            {
                statusReporter($"DrawLine Warning: Path from ({startPoint.X},{startPoint.Y}) towards ({tentativeEndPoint.X},{tentativeEndPoint.Y}) goes out of bounds (0-{logicalCanvasSize - 1}). Line not drawn. \n\nThe build failed. Fix the build errors and run again.");
               // statusReporter("\nThe build failed. Fix the build errors and run again.");//ponerlo rojo
                Debug.WriteLine($"DrawLine Warning: Path goes out of bounds. Line from ({startPoint.X},{startPoint.Y}) to ({tentativeEndPoint.X},{tentativeEndPoint.Y}) not drawn.");
                return true; // El comando se ejecutó (intentó), aunque la línea no se pintó.
            }

            Point endPoint = tentativeEndPoint;

            if (wallEState.BrushColor.ToString() != Brushes.Transparent.ToString()) // Comparación de IBrush puede ser tricky
            {
                var lineToAdd = new LineModel(startPoint, endPoint, wallEState.BrushColor, wallEState.PenThickness);
                linesToDraw.Add(lineToAdd);
                statusReporter($"Line drawn from ({startPoint.X},{startPoint.Y}) to ({endPoint.X},{endPoint.Y}).");
            }
            else
            {
                statusReporter($"Wall-E moved (transparently) from ({startPoint.X},{startPoint.Y}) to ({endPoint.X},{endPoint.Y}).");
            }
            
            wallEState.CurrentPosition = endPoint; // Mover Wall-E al punto final
            return true; // Comando ejecutado con éxito
        }
    }
}
