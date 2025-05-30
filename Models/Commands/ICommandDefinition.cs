// Models/Commands/ICommandDefinition.cs
using System.Collections.ObjectModel; // Para ObservableCollection
using PixelWallEApp.Models;       // Para WallEStateModel, LineModel
using System;                     // Para Action

namespace PixelWallEApp.Models.Commands
{
    public interface ICommandDefinition
    {
        /// <summary>
        /// Ejecuta el comando.
        /// </summary>
        /// <param name="wallEState">El estado actual de Wall-E.</param>
        /// <param name="linesToDraw">La colección donde se añaden las líneas visibles.</param>
        /// <param name="logicalCanvasSize">El tamaño lógico actual del canvas (para validación).</param>
        /// <param name="statusReporter">Una acción para reportar mensajes de estado o errores.</param>
        /// <returns>True si el comando se ejecutó (o fue válido), False si hubo un error que impidió la ejecución fundamental del comando (p.ej. parámetros incorrectos para el comando en sí).</returns>
        bool Execute(
            WallEStateModel wallEState,
            ObservableCollection<LineModel> linesToDraw,
            int logicalCanvasSize,
            Action<string> statusReporter
        );

        // Podrías añadir un método para parsear los argumentos si el comando los necesita
        // bool TryParseArguments(string[] args);
    }
}