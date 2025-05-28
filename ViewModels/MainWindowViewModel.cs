// ViewModels/MainWindowViewModel.cs
using Avalonia;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PixelWallEApp.Models; // Asegúrate que el namespace sea correcto
using System.Linq;
using System; // Para Math.Min y Convert.ToInt32 si es necesario
using System.Diagnostics;

namespace PixelWallEApp.ViewModels // Asegúrate que el namespace sea correcto
{
    public class MainWindowViewModel : ViewModelBase
    {
           private readonly WallEStateModel _wallE;
        public ObservableCollection<LineModel> Lines { get; }

        private int _logicalCanvasSize = 50;
        public int LogicalCanvasSize
        {
            get => _logicalCanvasSize;
            set
            {
                if (SetField(ref _logicalCanvasSize, value > 0 ? value : 1)) // Mínimo 1
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

        public MainWindowViewModel()
        {
            _wallE = new WallEStateModel();
            Lines = new ObservableCollection<LineModel>();

            RunTestDrawingCommand = new RelayCommand(ExecuteRunTestDrawing);
            ClearCanvasCommand = new RelayCommand(ExecuteClearCanvas);
            ResizeCanvasCommand = new RelayCommand(ExecuteResizeCanvas, CanExecuteResizeCanvas);

            _wallE.CurrentPosition = new Point(LogicalCanvasSize / 2, LogicalCanvasSize / 2);
            _wallE.PenThickness = 1;
        }

        private void ExecuteRunTestDrawing(object? parameter)
        {
            StatusMessage = "Running test drawing...";
            Spawn(10, 10);
            SetColor("Red");
            SetSize(1);
            DrawLine(1, 0, LogicalCanvasSize - 25); // Intentar dibujar hasta cerca del borde

            SetColor("Blue");
            SetSize(1);
            Spawn(5, LogicalCanvasSize - 5); // Cerca del borde inferior izquierdo
            DrawLine(1, 0, 10); // Esto podría salirse o no, dependiendo del tamaño

            SetColor("Green");
            Spawn(LogicalCanvasSize -5, LogicalCanvasSize -5);
            DrawLine(1,0,10); // Este seguro intenta salirse

            StatusMessage = "Test drawing finished. Check console for warnings.";
        }

        private void ExecuteClearCanvas(object? parameter)
        {
            Lines.Clear();
            _wallE.CurrentPosition = new Point(LogicalCanvasSize / 2, LogicalCanvasSize / 2);
            _wallE.BrushColor = Brushes.Transparent;
            _wallE.PenThickness = 1;
            StatusMessage = "Canvas cleared.";
        }

        private bool CanExecuteResizeCanvas(object? parameter)
        {
            return int.TryParse(NewCanvasSizeInput, out int size) && size > 0;
        }

        private void ExecuteResizeCanvas(object? parameter)
        {
            if (int.TryParse(NewCanvasSizeInput, out int size) && size > 0)
            {
                LogicalCanvasSize = size; // El setter ya se encarga de limpiar y notificar
            }
            else
            {
                StatusMessage = "Invalid size input.";
            }
        }

        private void Spawn(int x, int y)
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

        private void SetColor(string colorName)
        {
            _wallE.BrushColor = colorName.ToLower() switch
            {
                "red" => Brushes.Red, "blue" => Brushes.Blue, "green" => Brushes.Green,
                "yellow" => Brushes.Yellow, "orange" => Brushes.Orange, "purple" => Brushes.Purple,
                "black" => Brushes.Black, "white" => Brushes.White, "transparent" => Brushes.Transparent,
                _ => Brushes.Black
            };
        }

        private void SetSize(int k)
        {
            int newThickness = 1;
            if (k > 0)
            {
                newThickness = (k % 2 == 0) ? k - 1 : k;
                if (newThickness < 1) newThickness = 1;
            }
            _wallE.PenThickness = newThickness;
        }

        private void DrawLine(int dirX, int dirY, int distance)
        {
            if (!IsDirectionValid(dirX) || !IsDirectionValid(dirY) || distance <= 0)
            {
                StatusMessage = "DrawLine Error: Invalid direction or distance.";
                Debug.WriteLine(StatusMessage);
                return;
            }

            Point startPoint = _wallE.CurrentPosition;
            Point tentativeEndPoint = new Point(
                startPoint.X + (dirX * distance),
                startPoint.Y + (dirY * distance)
            );

            // Validar el punto final.
            // Si permitimos que la línea se dibuje parcialmente hasta el borde:
            int finalX = (int)tentativeEndPoint.X;
            int finalY = (int)tentativeEndPoint.Y;
            bool outOfBounds = false;

            if (finalX < 0 || finalX >= LogicalCanvasSize || finalY < 0 || finalY >= LogicalCanvasSize)
            {
                outOfBounds = true;
                StatusMessage = $"DrawLine Warning: Path from ({startPoint.X},{startPoint.Y}) towards ({tentativeEndPoint.X},{tentativeEndPoint.Y}) goes out of bounds (0-{LogicalCanvasSize-1}). Line not drawn.";
                Debug.WriteLine(StatusMessage);
                // Opción 1: No dibujar la línea y no mover Wall-E
                return;

                // Opción 2: Truncar la línea al borde y mover Wall-E al borde (más complejo)
                // Para esto, necesitarías recalcular la 'distance' o el 'finalX'/'finalY'
                // de manera que se quede en el borde. Por ahora, implementaremos la Opción 1.
            }
            
            // Si llegamos aquí, la línea está dentro de los límites
            Point endPoint = tentativeEndPoint;

            if (_wallE.BrushColor != Brushes.Transparent)
            {
                var lineToAdd = new LineModel(startPoint, endPoint, _wallE.BrushColor, _wallE.PenThickness);
                Lines.Add(lineToAdd);
                StatusMessage = $"Line drawn from ({startPoint.X},{startPoint.Y}) to ({endPoint.X},{endPoint.Y}).";
            }
            else
            {
                 StatusMessage = $"Wall-E moved (transparently) from ({startPoint.X},{startPoint.Y}) to ({endPoint.X},{endPoint.Y}).";
            }
            _wallE.CurrentPosition = endPoint; // Mover Wall-E al punto final (sea dibujado o no)
        }

        private bool IsDirectionValid(int dirValue)
        {
            return dirValue >= -1 && dirValue <= 1;
        }
    }
}