// ViewModels/MainWindowViewModel.cs
using Avalonia;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PixelWallEApp.Models;
using PixelWallEApp.Models.Commands;
using System.Linq;
using System;
using System.Diagnostics;

namespace PixelWallEApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public readonly WallEStateModel _wallE;
        public ObservableCollection<LineModel> Lines { get; }

        private int _logicalCanvasSize = 50;
        public int LogicalCanvasSize
        {
            get => _logicalCanvasSize;
            set
            {
                if (SetField(ref _logicalCanvasSize, value > 0 ? value : 1))
                {
                    ExecuteClearCanvas(null); // Llama al método que usa comandos
                    ReportCommandStatus($"Canvas resized to {value}x{value}.");
                }
            }
        }
        public void ExecuteClearCanvas(object? parameter)
        {
            Lines.Clear();
            _wallE.CurrentPosition = new Point(LogicalCanvasSize / 2, LogicalCanvasSize / 2);
            _wallE.BrushColor = Brushes.Transparent;
            _wallE.PenThickness = 1;
            ReportCommandStatus("Canvas cleared.");
        }

        private string _newCanvasSizeInput = "50";
        public string NewCanvasSizeInput
        {
            get => _newCanvasSizeInput;
            set
            {
                if (SetField(ref _newCanvasSizeInput, value))
                {
                    (ResizeCanvasCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private string _statusMessage = "Ready.";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public ICommand RunTestDrawingCommand { get; }
        public ICommand ClearCanvasCommand { get; }
        public ICommand ResizeCanvasCommand { get; }


        public MainWindowViewModel()
        {
            _wallE = new WallEStateModel();
            Lines = new ObservableCollection<LineModel>();

            RunTestDrawingCommand = new RelayCommand(ExecuteRunTestDrawing);
            ClearCanvasCommand = new RelayCommand(ExecuteClearCanvasAction); // Cambiado para distinguir
            ResizeCanvasCommand = new RelayCommand(ExecuteResizeCanvasAction, CanExecuteResizeCanvas); // Cambiado

            // Inicializar Wall-E
            var initialSpawnCmd = new SpawnCommand(LogicalCanvasSize / 2, LogicalCanvasSize / 2);
            initialSpawnCmd.Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);

            var initialSizeCmd = new SetPenSizeCommand(1);
            initialSizeCmd.Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
        }

        private void ReportCommandStatus(string message)
        {
            StatusMessage = message;
        }

        public void ExecuteRunTestDrawing(object? parameter)
        {
            ReportCommandStatus("Running test drawing...");

            // Usar los nuevos comandos
            new SpawnCommand(10, 10).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            new SetColorCommand("Red").Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            new SetPenSizeCommand(1).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            new DrawLineCommand(1, 0, LogicalCanvasSize - 25).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);

            new SetColorCommand("Blue").Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            new SetPenSizeCommand(1).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus); // PenSize ya fue definido
            new SpawnCommand(5, LogicalCanvasSize - 5).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            new DrawLineCommand(1, 0, 10).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);

            // new SetColorCommand("Green").Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            // new SpawnCommand(LogicalCanvasSize - 5, LogicalCanvasSize - 5).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            // new DrawLineCommand(1, 0, 10).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus); // Este intentará salirse

            // El StatusMessage se actualizará por la última llamada a ReportCommandStatus
            // Puedes añadir un mensaje general si lo deseas:
            // ReportCommandStatus("Test drawing finished. Check console for warnings/errors.");
        }

        // Este es el método que el ICommand "ClearCanvasCommand" realmente llama
        private void ExecuteClearCanvasAction(object? parameter)
        {
            Lines.Clear();
            // Re-spawn y resetear el pincel usando comandos
            new SpawnCommand(LogicalCanvasSize / 2, LogicalCanvasSize / 2).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            new SetColorCommand("Transparent").Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            new SetPenSizeCommand(1).Execute(_wallE, Lines, LogicalCanvasSize, ReportCommandStatus);
            ReportCommandStatus("Canvas cleared.");
        }

        public bool CanExecuteResizeCanvas(object? parameter)
        {
            return int.TryParse(NewCanvasSizeInput, out int size) && size > 0;
        }

        // Este es el método que el ICommand "ResizeCanvasCommand" realmente llama
        private void ExecuteResizeCanvasAction(object? parameter)
        {
            if (int.TryParse(NewCanvasSizeInput, out int size) && size > 0)
            {
                // La propiedad LogicalCanvasSize al ser cambiada ya llama a ExecuteClearCanvasAction (indirectamente)
                // y actualiza el StatusMessage.
                LogicalCanvasSize = size;
            }
            else
            {
                ReportCommandStatus("Invalid size input for resize.");
            }
        }
    }
}

// Los métodos Spawn, SetColor, SetSize ya no son necesarios como métodos públicos directos aquí

