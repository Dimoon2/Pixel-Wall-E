using Avalonia.Threading; // For Dispatcher
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PixelWallEApp.Models;
using PixelWallEApp.Models.Commands;
using PixelWallEApp.Views.Controls; // For PixelCanvas reference (needed for redraw)
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // For async relay command

namespace PixelWallEApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _codeText = @"// Enter Wall-E code here
// Example:
Spawn(0, 0)
DrawLine(1, 1, 5) // Draw diagonal down-right
// Color(Red) // Not implemented yet
// Size(3) // Not implemented yet
// DrawLine(1, 0, 10) // Draw right
";

        [ObservableProperty]
        private string _outputLog = "Output will appear here.";

        [ObservableProperty]
        private int _canvasSizeInput = 50; // Default size for input field

        [ObservableProperty]
        private CanvasState _canvasState;

        [ObservableProperty]
        private WallEState _wallEState;

        // Reference to the actual canvas control to trigger redraws
        // This breaks pure MVVM slightly but is pragmatic for custom drawing controls.
        // An alternative is using messaging or events.
        public PixelCanvas? CanvasControl { get; set; }


        public MainWindowViewModel()
        {
            _wallEState = new WallEState();
            // Initialize CanvasState with the default input size
            _canvasState = new CanvasState(_canvasSizeInput);
            // Set the static reference in App for the canvas rendering
             App.MainWindowViewModel = this; // Make VM accessible
        }

        [RelayCommand]
        private void ResizeCanvas()
        {
            LogOutput($"Resizing canvas to {CanvasSizeInput}x{CanvasSizeInput}...");
            CanvasState.Resize(CanvasSizeInput);
            // Explicitly trigger redraw on the canvas control
            CanvasControl?.InvalidateVisual();
            LogOutput($"Canvas resized. Cleared to white.");
        }

        [RelayCommand]
        private async Task ExecuteCode() // Make async if parsing/execution could be long
        {
            if (CanvasControl == null)
            {
                LogOutput("Error: Canvas control not available.");
                return;
            }

            LogOutput("Parsing code...");
            List<ICommandDefinition>? commands = null;
            try
            {
                commands = CommandParser.Parse(CodeText);
                LogOutput($"Parsing successful. {commands.Count} command(s) found.");
            }
            catch (FormatException ex)
            {
                LogOutput($"Parsing Error: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                 LogOutput($"Unexpected Parsing Error: {ex.Message}");
                 return;
            }


            LogOutput("Executing code...");
            string? executionResult = null;
            try
            {
                 // Run the interpreter - it modifies WallEState and CanvasState directly
                executionResult = Interpreter.Run(commands, WallEState, CanvasState);
            }
            catch (Exception ex)
            {
                 LogOutput($"Unexpected Execution Error: {ex.Message}");
                 // Optionally reset state or leave canvas partially drawn?
                 // Let's leave it partially drawn for debugging.
                 CanvasControl?.InvalidateVisual(); // Redraw partially modified state
                 return;
            }


            if (executionResult != null)
            {
                LogOutput($"Execution Failed: {executionResult}");
                 // Redraw canvas to show state *before* the failed command (if needed, depends on Interpreter logic)
                 // Currently, Interpreter stops, so the state is as it was *after* the last successful command.
                 CanvasControl?.InvalidateVisual();
            }
            else
            {
                LogOutput("Execution completed successfully.");
                // Trigger redraw on the UI thread after successful execution
                 await Dispatcher.UIThread.InvokeAsync(() => CanvasControl?.InvalidateVisual());

            }
        }

        private void LogOutput(string message)
        {
            // Prepend new messages to the log
            OutputLog = $"{DateTime.Now:HH:mm:ss}: {message}\n{OutputLog}";
            // Optional: Limit log length
            if (OutputLog.Length > 4000) OutputLog = OutputLog.Substring(0, 4000);
        }

         // Expose WallEState for the canvas rendering (simple approach)
         public WallEState? WallE => _wallEState;
    }
}