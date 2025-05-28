using Avalonia; // Para Point
using Avalonia.Media; // Para IBrush

namespace PixelWallEApp.Models
{
    public class LineModel
    {
        public Point Start { get; } // X, Y serán tratados como enteros
        public Point End { get; }   // X, Y serán tratados como enteros
        public IBrush Brush { get; }
        public int Thickness { get; } // Cambiado a int

        public LineModel(Point start, Point end, IBrush brush, int thickness)
        {
            Start = start;
            End = end;
            Brush = brush;
            Thickness = thickness;
        }
    }
}