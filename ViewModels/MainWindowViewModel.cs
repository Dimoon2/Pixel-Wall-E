using Avalonia.Threading; // For Dispatcher
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PixelWallEApp.Models;
using PixelWallEApp.Models.Canvas;
using PixelWallEApp.Views; // For PixelCanvas reference (needed for redraw)
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // For async relay command
using AvaloniaEdit.Document;
using Interpreter.Core; // Necesario para TextDocument
using Interpreter.Core.Interpreter;
using Interpreter.Core.Ast.Statements;
using Avalonia.Media;
using System.Drawing;
using Interpreter.Core.Interpreter.Helpers;
using System.Diagnostics;

namespace PixelWallEApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _initialCodeText = @"
Spawn(4, 4)
Color(Purple)
n<- (4+3)**2/4
DrawLine(1, 1, n)";
        [ObservableProperty]
        private TextDocument _theDocument;

        [ObservableProperty]
        private string _outputLog = "Output will appear here.";

        [ObservableProperty]
        private int _canvasSizeInput = 50; // Default size for input field

        [ObservableProperty]
        private CanvasState _canvasState;

        [ObservableProperty]
        private WallEState _wallEState;
        public bool IsWallESpawned => WallEX >= 0 && WallEY >= 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsWallESpawned))]
        private int _wallEX = -1;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsWallESpawned))]
        private int _wallEY = -1;

        // Reference to the actual canvas control to trigger redraws
        // This breaks pure MVVM slightly but is pragmatic for custom drawing controls.
        // An alternative is using messaging or events.


        public MainWindowViewModel()
        {
            _wallEState = new WallEState();
            // Initialize CanvasState with the default input size
            _canvasState = new CanvasState(_canvasSizeInput);
            // Set the static reference in App for the canvas rendering
            _theDocument = new TextDocument(_initialCodeText); // <--- CAMBIO: Inicializa el documento
            WallELocation();

        }
        private void WallELocation()
        {
            WallEX = _wallEState.X;
            WallEY = _wallEState.Y;
        }

        [RelayCommand]
        private void ResizeCanvas()
        {
            LogOutput($"Resizing canvas to {CanvasSizeInput}x{CanvasSizeInput}...");
            CanvasState.Resize(CanvasSizeInput);
            LogOutput($"Canvas resized. Cleared to white.");
        }

        [RelayCommand]
        private void Clear()
        {
            LogOutput($"Clearing canvas...");
            CanvasState.Clear(FunctionHandlers.GetColor("white"));
            LogOutput($"Canvas clead, evrything up to default");
        }

        [RelayCommand]
        private async Task ExecuteCode()
        {

            string codeToExecute = TheDocument.Text;
            LogOutput($"Attempting to parse and execute:\n{codeToExecute.Substring(0, Math.Min(codeToExecute.Length, 100))}..."); // Loguea el inicio del código

            // Asumiendo que tu Lexer, Parser e Interprete pueden lanzar excepciones en caso de error
            // o que llenan una lista de errores que compruebas.
            Lexer lexer = new Lexer(codeToExecute);
            List<Token> tokens = lexer.Tokenize();
            Debug.WriteLine("Tokens");
            for (int i = 0; i < tokens.Count; i++)
            {
                Debug.WriteLine($"{tokens[i]}");
            }
            // Aquí podrías loguear el número de tokens o algunos de ellos para depuración.

            Parser parser = new Parser(tokens);
            // ProgramNode source = parser.ParseProgram(); // ParseProgram DEBERÍA lanzar error o devolver null/lista de errores

            // Ejemplo de manejo de errores del parser si devuelve una lista de errores
            List<string> parsingErrors = parser.errors; // Asumiendo que `errors` es una propiedad pública
            if (parsingErrors != null && parsingErrors.Count > 0)
            {
                LogOutput("Parsing failed with errors:");
                foreach (var error in parsingErrors)
                {
                    LogOutput($" - {error}");
                }
                return;
            }
            ProgramNode source = parser.ParseProgram(); // Si no hay errores, parsea

            if (source == null && (parsingErrors == null || parsingErrors.Count == 0))
            {
                LogOutput("Parsing resulted in null program without explicit errors. Check parser logic.");
                return;
            }


            LogOutput("Parsing successful. Executing program...");
            Interprete interpreter = new Interprete(_canvasState, _wallEState); // Pasa las dependencias
                                                                                // Aquí, tu método ExecuteProgram también podría lanzar excepciones o tener su propio manejo de errores.
            interpreter.ExecuteProgram(source);

            List<string> Errors = interpreter.ErrorLog; // Asumiendo que `errors` es una propiedad pública
            if (Errors != null && Errors.Count > 0)
            {
                LogOutput("Parsing failed with errors:");
                foreach (var error in Errors)
                {
                    LogOutput($" - {error}");
                }
                return;
            }
            // Si ExecuteProgram modifica una lista de errores en el intérprete:
            CanvasState.NotifyChanged(); // Force UI update
            WallELocation();      // Update Wall-E pos in VM if displayed) { /* Loguear errores */ }

            LogOutput($"Execution finished (or attempted), Walle current position:({_wallEState.X} , {_wallEState.Y})");
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