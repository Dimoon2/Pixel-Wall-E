// Models/Commands/SpawnCommand.cs
using Avalonia; // Para Point
using System.Collections.ObjectModel;
using PixelWallEApp.Models; // Para WallEStateModel, LineModel (aunque LineModel no se usa aquí directamente)
using System;
using System.Diagnostics;

namespace PixelWallEApp.Models.Commands
{
    public class SpawnCommand : ICommandDefinition
    {
        private readonly int x;
        private readonly int y;

        public SpawnCommand(int x, int y)
        {
            x = x;
            y = y;
        }

        public bool Execute(
            WallEStateModel wallEState,
            ObservableCollection<LineModel> linesToDraw, // No se usa para Spawn, pero es parte de la interfaz
            int logicalCanvasSize,
            Action<string> statusReporter)
        {
            if (x >= 0 && x < logicalCanvasSize && y >= 0 && y < logicalCanvasSize)
            {
                wallEState.CurrentPosition = new Point(x, y);
                statusReporter($"Wall-E spawned at ({x},{y}).");
                return true;
            }
            else
            {
                string msg = $"SpawnCommand Error: Position ({x},{y}) is outside canvas (0-{logicalCanvasSize - 1}). Wall-E not moved.";
                statusReporter(msg);
                statusReporter("\nThe build failed. Fix the build errors and run again.");//ponerlo rojo
                Debug.WriteLine(msg);
                return false; // El comando en sí falló en su validación principal (spawn fuera de límites)
                              // Podrías decidir que esto es 'true' si el comando se "intentó" pero no tuvo efecto.
                              // Lo marco como 'false' para indicar que la operación de spawn no fue válida.
            }
        }
    }
}
