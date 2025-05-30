// Models/Commands/SetColorCommand.cs
using Avalonia.Media; // Para Brushes e IBrush
using System.Collections.ObjectModel;
using PixelWallEApp.Models;
using System;
using System.Linq; // Para ToLower

namespace PixelWallEApp.Models.Commands
{
    public class SetColorCommand : ICommandDefinition
    {
        private readonly string _colorName;

        public SetColorCommand(string colorName)
        {
            _colorName = colorName ?? "black"; // Un valor por defecto si es nulo
        }

        public bool Execute(
            WallEStateModel wallEState,
            ObservableCollection<LineModel> linesToDraw, // No se usa
            int logicalCanvasSize,                      // No se usa
            Action<string> statusReporter)
        {
            IBrush? newBrush = _colorName.ToLower() switch
            {
                "red" => Brushes.Red,
                "blue" => Brushes.Blue,
                "green" => Brushes.Green,
                "yellow" => Brushes.Yellow,
                "orange" => Brushes.Orange,
                "purple" => Brushes.Purple,
                "black" => Brushes.Black,
                "white" => Brushes.White,         // Borrador
                "transparent" => Brushes.Transparent, // No dibuja
                _ => null // Color no reconocido
            };

            if (newBrush != null)
            {
                wallEState.BrushColor = newBrush;
                statusReporter($"Color set to {_colorName}.");
                return true;
            }
            else
            {
                statusReporter($"SetColorCommand Error: Unknown color '{_colorName}'. Color not changed.");
                statusReporter("\nThe build failed. Fix the build errors and run again.");//ponerlo rojo
                // Opcionalmente, podrías establecer un color por defecto aquí, por ejemplo, negro.
                // wallEState.BrushColor = Brushes.Black;
                // statusReporter($"Color set to default (black) due to unknown color '{_colorName}'.");
                return false; // El comando falló porque el color no es válido
            }
        }
    }
}
