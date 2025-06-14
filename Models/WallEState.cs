using Avalonia.Media; // For Color

namespace PixelWallEApp.Models
{
    public class WallEState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color BrushColor { get; set; } = Colors.Transparent; // Default according to spec
        public int BrushSize { get; set; } = 1; // Default according to spec
        public bool IsSpawned { get; set; } = false; // Track if Spawn has been called

        public void Reset()
        {
            X = 0;
            Y = 0;
            BrushColor = Colors.Transparent;
            BrushSize = 1;
            IsSpawned = false;
        }
    }
}