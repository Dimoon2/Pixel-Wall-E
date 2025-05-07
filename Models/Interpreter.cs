using PixelWallEApp.Models.Commands;
using System.Collections.Generic;
using System;

namespace PixelWallEApp.Models
{
    public static class Interpreter
    {
        // Returns error message if execution fails, null otherwise
        public static string? Run(List<ICommandDefinition> commands, WallEState wallEState, CanvasState canvasState)
        {
             // Spec: "Todo código válido debe, obligatoriamente, comenzar con un comando Spawn"
             // Spec: "sólo puede utilizarse esta instrucción una vez"
             // We'll enforce the 'first command' rule here.
             // The 'only once' rule is implicitly handled if parsing only allows one Spawn.
             // A more robust parser/interpreter would track if Spawn was already called.

            if (commands.Count == 0)
            {
                return "Execution Error: No commands to execute.";
            }

            if (!(commands[0] is SpawnCommand))
            {
                return "Execution Error: The first command must be Spawn(x, y).";
            }

            // Reset Wall-E state before execution, *except* the canvas pixels
            // The spec implies the canvas state persists between runs unless resized.
            wallEState.Reset();

            for (int i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                string? error = null;

                 // Extra check: Ensure Spawn isn't used again after the first command
                 if (i > 0 && command is SpawnCommand)
                 {
                      return $"Runtime Error (Command {i + 1}): Spawn command can only be used once at the beginning.";
                 }

                try
                {
                    error = command.Execute(wallEState, canvasState);
                }
                catch (Exception ex)
                {
                     // Catch unexpected errors during command execution
                     error = $"Runtime Error (Command {i + 1}): Unexpected error executing {command.GetType().Name}: {ex.Message}";
                }


                if (error != null)
                {
                    // Stop execution on the first error encountered
                    return $"Error on command {i + 1} ({command.GetType().Name}): {error}";
                }
            }

            return null; // Success
        }
    }
}