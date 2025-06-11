using Interpreter.Core;
using System.Text;

namespace Interpreter.Core.Interpreter.Canvas
{
    public class WallEContext
    {
        public int CurrentX { get; set; }
        public int CurrentY { get; set; }
        public PixelColor CurrentBrushColor { get; set; }
        public int CurrentBrushSize { get; set; }

        public WallEContext()
        {
            CurrentX = 0;
            CurrentY = 0;
            CurrentBrushColor = PixelColor.Black;
            CurrentBrushSize = 1;
        }

        public void Reset()
        {
            CurrentX = 0;
            CurrentY = 0;
            CurrentBrushColor = PixelColor.Black;
            CurrentBrushSize = 1;
        }

        public override string ToString()
        {
            return $"Wall-E: Pos=({CurrentX},{CurrentY}), Color={CurrentBrushColor}, Size={CurrentBrushSize}";
        }
    }
}