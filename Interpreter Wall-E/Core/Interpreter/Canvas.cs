using Interpreter.Core;
using System.Text;

namespace Interpreter.Core.Interpreter
{
    public class PixelColor
    {
        public string Name { get; }
        public PixelColor(string name)
        {
            Name = name?.ToLower() ?? "default";
        }
        public override string ToString() => Name;
        public override bool Equals(object obj)//////////???????
        {
            return obj is PixelColor other && Name == other.Name;
        }

        public override int GetHashCode() => Name.GetHashCode();///////////////????????????

        public static bool operator ==(PixelColor a, PixelColor b)
        {
            if (ReferenceEquals(a, null)) return ReferenceEquals(b, null);
            return a.Equals(b);
        }
        public static bool operator !=(PixelColor a, PixelColor b) => !(a == b);

        // Predefined common colors (can be extended)
        public static PixelColor White { get; } = new PixelColor("white");
        public static PixelColor Black { get; } = new PixelColor("black");
        public static PixelColor Red { get; } = new PixelColor("red");
        public static PixelColor Green { get; } = new PixelColor("green");
        public static PixelColor Blue { get; } = new PixelColor("blue");
    }

    public class Canvas
    {
        private PixelColor[,] pixels;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public PixelColor DefaultBackgroundColor { get; set; } = PixelColor.White;

        public void Initialize(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                // Or throw an argument exception
                Console.Error.WriteLine("Canvas dimensions must be positive.");
                // Default to a small canvas if initialization fails this way
                Width = 10;
                Height = 10;
            }
            else
            {
                Width = width;
                Height = height;
            }
            pixels = new PixelColor[Width, Height];
            Clear();
        }

        public void Clear()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    pixels[x, y] = DefaultBackgroundColor;
                }
            }
        }

        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void SetPincel(int x, int y, PixelColor color)
        {
            if (IsValidCoordinate(x, y))
            {
                pixels[x, y] = color ?? DefaultBackgroundColor;
            }
            else
            {
                // Optional: Log an error or warning if trying to draw out of bounds
                 Console.Error.WriteLine($"Warning: Attempt to draw at out-of-bounds coordinate ({x},{y}). Ignoring.");
            }
        }

        public PixelColor GetPixel(int x, int y)
        {
            if (IsValidCoordinate(x, y))
            {
                return pixels[x, y];
            }
            return null!;
        }

        // Method to print the canvas to the console (for debugging)
        public void PrintToConsole()
        {
            Console.WriteLine($"Canvas ({Width}x{Height}):");
            var sb = new StringBuilder();
            for (int y = 0; y < Height; y++) // Typically iterate Y (rows) outer, X (cols) inner
            {
                for (int x = 0; x < Width; x++)
                {
                    PixelColor color = pixels[x, y];
                    // Use the first letter of the color name, or a default for unknown/null
                    char displayChar = color?.Name.Length > 0 ? char.ToUpper(color.Name[0]) : '.';
                    if (color == PixelColor.White) displayChar = '.'; // explicit for white
                    else if (color == PixelColor.Black) displayChar = '#'; // explicit for black

                    sb.Append(displayChar);
                    sb.Append(' '); // Add a space for better readability
                }
                sb.AppendLine();
            }
            Console.Write(sb.ToString());
        }
    }
}