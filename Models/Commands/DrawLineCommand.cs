// Commands/DrawLineCommand.cs
using Avalonia; // Para Point
using PixelWallEApp.Models;
using PixelWallEApp.ViewModels; // Para MainWindowViewModel
using System.Diagnostics; // Para Debug.WriteLine
using PixelWallEApp.Models.Commands;

namespace PixelWallEApp.Commands
{
    public class DrawLineCommand : ICommandDefinition
    {
        public string CommandName => "DrawLine";

        public void Execute(MainWindowViewModel viewModel, params object[] parameters)
        {
            if (parameters.Length < 3)
            {
                viewModel.StatusMessage = "DrawLineCommand Error: Insufficient parameters (expected dirX, dirY, distance).";
                Debug.WriteLine(viewModel.StatusMessage);
                return;
            }

            if (!(parameters[0] is int dirX) ||
                !(parameters[1] is int dirY) ||
                !(parameters[2] is int distance))
            {
                viewModel.StatusMessage = "DrawLineCommand Error: Invalid parameter types.";
                Debug.WriteLine(viewModel.StatusMessage);
                return;
            }

            // --- INICIO DE LA LÓGICA COPIADA Y ADAPTADA DE MainWindowViewModel.DrawLine ---
            if (!IsDirectionValid.DirectionValid(dirX) || !IsDirectionValid.DirectionValid(dirY) || distance <= 0)
            {
                viewModel.StatusMessage = "DrawLine Error: Invalid direction or distance.";
                Debug.WriteLine(viewModel.StatusMessage);
                return;
            }

            // Acceder al estado de Wall-E y al canvas size a través del viewModel
            Point startPoint = viewModel._wallE.CurrentPosition; // Acceso directo al campo _wallE
            Point tentativeEndPoint = new Point(
                startPoint.X + (dirX * distance),
                startPoint.Y + (dirY * distance)
            );

            int finalX = (int)tentativeEndPoint.X;
            int finalY = (int)tentativeEndPoint.Y;

            if (finalX < 0 || finalX >= viewModel.LogicalCanvasSize || 
                finalY < 0 || finalY >= viewModel.LogicalCanvasSize)
            {
                viewModel.StatusMessage = $"DrawLine Warning: Path from ({startPoint.X},{startPoint.Y}) towards ({tentativeEndPoint.X},{tentativeEndPoint.Y}) goes out of bounds (0-{viewModel.LogicalCanvasSize-1}). Line not drawn.";
                Debug.WriteLine(viewModel.StatusMessage);
                return; 
            }
            
            Point endPoint = tentativeEndPoint;

            if (viewModel._wallE.BrushColor != Avalonia.Media.Brushes.Transparent)
            {
                // Usar las propiedades de _wallE del viewModel
                var lineToAdd = new LineModel(startPoint, endPoint, viewModel._wallE.BrushColor, viewModel._wallE.PenThickness);
                viewModel.Lines.Add(lineToAdd); // Acceso a la colección Lines del viewModel
                viewModel.StatusMessage = $"Line drawn from ({startPoint.X},{startPoint.Y}) to ({endPoint.X},{endPoint.Y}).";
            }
            else
            {
                 viewModel.StatusMessage = $"Wall-E moved (transparently) from ({startPoint.X},{startPoint.Y}) to ({endPoint.X},{endPoint.Y}).";
            }
            
            // Actualizar la posición de Wall-E en el viewModel
            viewModel._wallE.CurrentPosition = endPoint;
            // --- FIN DE LA LÓGICA COPIADA Y ADAPTADA ---
        }

        // Método auxiliar (puede ser estático o parte de una clase de utilidad si se comparte)
        // private bool IsDirectionValid(int dirValue)
        // {
        //     return dirValue >= -1 && dirValue <= 1;
        // }
    }
}