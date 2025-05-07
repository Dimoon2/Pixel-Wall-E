using Avalonia.Media;
using System.ComponentModel; // For INotifyPropertyChanged (optional but good)
using System.Runtime.CompilerServices; // For CallerMemberName

namespace PixelWallEApp.Models
{
    // Simple implementation. Could be improved with ObservableCollection for rows
    // if binding directly, but direct drawing is often simpler/faster for large grids.
    public class CanvasState : INotifyPropertyChanged
    {
        private Color[,] _pixels;
        private int _size;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Size
        {
            get => _size;
            private set // Keep setter private, resizing managed by Resize method
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged(); // Notify UI if bound
                }
            }
        }

        // Read-only access to pixels for drawing
        public Color[,] Pixels => _pixels;

        public CanvasState(int initialSize = 50) // Default size
        {
            _pixels = new Color[0, 0]; // Initial empty state
            Resize(initialSize);
        }

        public void Resize(int newSize)
        {
            if (newSize <= 0) newSize = 1; // Prevent zero or negative size

            Size = newSize;
            _pixels = new Color[Size, Size];
            Clear(); // Clear canvas on resize (as per spec)
            OnPropertyChanged(nameof(Pixels)); // Notify that the data structure changed
        }

        public void Clear()
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    _pixels[x, y] = Colors.White; // <-- Asegúrate que sea White
                }
            }
            // No need to trigger PropertyChanged here unless something binds directly to individual pixel changes
        }

        public Color GetPixel(int x, int y)
        {
            if (x >= 0 && x < Size && y >= 0 && y < Size)
            {
                return _pixels[x, y];
            }
            return Colors.Transparent; // Or throw an exception, depending on desired behavior
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x >= 0 && x < Size && y >= 0 && y < Size)
            {
                _pixels[x, y] = color;
                // Note: We won't trigger PropertyChanged for individual pixels for performance.
                // The canvas control will be explicitly told to redraw.
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}