// Models/Commands/SetPenSizeCommand.cs
using System.Collections.ObjectModel;
using PixelWallEApp.Models;
using System;

namespace PixelWallEApp.Models.Commands
{
    public class SetPenSizeCommand : ICommandDefinition
    {
        private readonly int _size;

        public SetPenSizeCommand(int size)
        {
            _size = size;
        }

        public bool Execute(
            WallEStateModel wallEState,
            ObservableCollection<LineModel> linesToDraw, // No se usa
            int logicalCanvasSize,                      // No se usa
            Action<string> statusReporter)
        {
            if (_size <= 0)
            {
                // Considerar si un tamaño inválido debe ser un error o ajustarse a un mínimo.
                // La especificación original no detalla el manejo de k<=0 para "size k".
                // Asumiremos que se ajusta a 1, como antes.
                wallEState.PenThickness = 1;
                statusReporter($"Pen size {_size} is invalid, set to minimum (1).");
            }
            else
            {
                // Si es par, usar el impar inmediatamente menor.
                int newThickness = (_size % 2 == 0) ? _size - 1 : _size;
                if (newThickness < 1) newThickness = 1; // Asegurar que no sea menor que 1

                wallEState.PenThickness = newThickness;
                statusReporter($"Pen size set to {newThickness}. (Input was {_size})");
            }
            return true; // El comando siempre se "ejecuta" ajustando el tamaño.
        }
    }
}
