// ViewModels/MainWindowViewModel.cs
using Avalonia;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PixelWallEApp.Models;
using PixelWallEApp.Commands; // ¡Añadir using para los comandos!
using System.Linq;
using System;
using System.Diagnostics;

namespace PixelWallEApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Hacer _wallE público para que los comandos puedan accederlo (o crear propiedades/métodos)
        public readonly WallEStateModel _wallE; 
        public ObservableCollection<LineModel> Lines { get; }

        // ... (LogicalCanvasSize, NewCanvasSizeInput, StatusMessage y Commands sin cambios) ...
        private int _logicalCanvasSize = 50;
        public int LogicalCanvasSize
        {
            get => _logicalCanvasSize;
            set
            {
                if (SetField(ref _logicalCanvasSize, value > 0 ? value : 1)) 
                {
                    ExecuteClearCanvas(null);
                    StatusMessage = $"Canvas resized to {value}x{value}.";
                }
            }
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

        // Instancia del comando DrawLine
        private readonly DrawLineCommand _drawLineCmdInstance;

        public MainWindowViewModel()
        {
            _wallE = new WallEStateModel();
            Lines = new ObservableCollection<LineModel>();

            // Crear instancia del comando
            _drawLineCmdInstance = new DrawLineCommand();

            RunTestDrawingCommand = new RelayCommand(ExecuteRunTestDrawing);
            ClearCanvasCommand = new RelayCommand(ExecuteClearCanvas);
            ResizeCanvasCommand = new RelayCommand(ExecuteResizeCanvas, CanExecuteResizeCanvas);

            _wallE.CurrentPosition = new Point(LogicalCanvasSize / 2, LogicalCanvasSize / 2);
            _wallE.PenThickness = 1;
        }

        public void ExecuteRunTestDrawing(object? parameter)
        {
            StatusMessage = "Running test drawing...";
            Spawn(10, 10);
            SetColor("Red");
            SetSize(1);
            // Usar el comando DrawLine
            _drawLineCmdInstance.Execute(this, 1, 0, LogicalCanvasSize - 25);

            SetColor("Blue");
            SetSize(1);
            Spawn(5, LogicalCanvasSize - 5);
            _drawLineCmdInstance.Execute(this, 1, 0, 10);

            SetColor("Green");
            Spawn(LogicalCanvasSize -5, LogicalCanvasSize -5);
            _drawLineCmdInstance.Execute(this, 1, 0, 10);

            StatusMessage = "Test drawing finished. Check console for warnings.";
        }

        // ... (ExecuteClearCanvas, CanExecuteResizeCanvas, ExecuteResizeCanvas sin cambios) ...
        public void ExecuteClearCanvas(object? parameter)
        {
            Lines.Clear();
            _wallE.CurrentPosition = new Point(LogicalCanvasSize / 2, LogicalCanvasSize / 2);
            _wallE.BrushColor = Brushes.Transparent;
            _wallE.PenThickness = 1;
            StatusMessage = "Canvas cleared.";
        }

        public bool CanExecuteResizeCanvas(object? parameter)
        {
            return int.TryParse(NewCanvasSizeInput, out int size) && size > 0;
        }

        public void ExecuteResizeCanvas(object? parameter)
        {
            if (int.TryParse(NewCanvasSizeInput, out int size) && size > 0)
            {
                LogicalCanvasSize = size;
            }
            else
            {
                StatusMessage = "Invalid size input.";
            }
        }


        // Los métodos Spawn, SetColor, SetSize pueden permanecer aquí o también convertirse en comandos
        public void Spawn(int x, int y)
        {
            if (x >= 0 && x < LogicalCanvasSize && y >= 0 && y < LogicalCanvasSize)
            {
                _wallE.CurrentPosition = new Point(x, y);
                StatusMessage = $"Wall-E spawned at ({x},{y}).";
            }
            else
            {
                StatusMessage = $"Spawn Error: Position ({x},{y}) is outside canvas (0-{LogicalCanvasSize-1}). Wall-E not moved.";
                Debug.WriteLine(StatusMessage);
            }
        }

        public void SetColor(string colorName)
        {
            _wallE.BrushColor = colorName.ToLower() switch
            {
                "red" => Brushes.Red, "blue" => Brushes.Blue, "green" => Brushes.Green,
                "yellow" => Brushes.Yellow, "orange" => Brushes.Orange, "purple" => Brushes.Purple,
                "black" => Brushes.Black, "white" => Brushes.White, "transparent" => Brushes.Transparent,
                _ => Brushes.Black
            };
        }

        public void SetSize(int k)
        {
            int newThickness = 1;
            if (k > 0)
            {
                newThickness = (k % 2 == 0) ? k - 1 : k;
                if (newThickness < 1) newThickness = 1;
            }
            _wallE.PenThickness = newThickness;
        }

        // El método DrawLine original ya no está aquí, fue movido a DrawLineCommand.
        // El método IsDirectionValid puede permanecer o ser movido a una clase de utilidad
        // si es usado por múltiples comandos. Por ahora, lo dejé en DrawLineCommand.
    }
}