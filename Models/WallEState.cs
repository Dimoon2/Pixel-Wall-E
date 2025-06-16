using Avalonia.Media; // For Color

namespace PixelWallEApp.Models
{
    public class WallEState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color BrushColor { get; set; } = Colors.Transparent; 
        public int BrushSize { get; set; } = 1;
        public bool IsSpawned { get; set; } = false;

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