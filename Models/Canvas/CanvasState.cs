// Models/PixelCanvas.cs
using Avalonia.Media;
using Interpreter.Core.Interpreter;
using System;
using System.Diagnostics.CodeAnalysis; // Needed for MemberNotNull attribute

namespace PixelWallEApp.Models.Canvas;

public class CanvasState
{
    private Color[,] _pixels; 

    public int Size { get; private set; }

    public event EventHandler? CanvasInvalidated; 

    
    public CanvasState(int size, WallEState wallE)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size) + "/" + nameof(size), "Canvas dimensions must be positive.");
        }

        Size = size;
        _pixels = new Color[Size, Size];

        Clear(Colors.White, wallE);
    }

    public void Resize(int newSize,WallEState wallE)
    {
        if (newSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newSize) + "/" + nameof(newSize), "Canvas dimensions must be positive.");
        }

        if (newSize != Size)
        {
            Size = newSize;
            _pixels = new Color[newSize, newSize]; // Reallocate
            Clear(Colors.White, wallE); // Clear the new array
        }
    }

    public void Clear(Color color, WallEState wallE)
    {
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                _pixels[x, y] = color;
            }
        }
        wallE.IsSpawned = false;
        NotifyChanged(); // Notify after clearing
    }

    public bool SetPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < Size && y >= 0 && y < Size)
        {
            _pixels[x, y] = color;
            return true; // Pixel was set
        }
        return false; // Pixel out of bounds
    }

    // Get a single pixel color
    public Color GetPixel(int x, int y)
    {
        if (x >= 0 && x < Size && y >= 0 && y < Size)
        {
            return _pixels[x, y];
        }
        return Colors.Transparent;
    }


    public Color[,] GetPixelsData()
    {
        return _pixels;
    }

    public void NotifyChanged()
    {
        CanvasInvalidated?.Invoke(this, EventArgs.Empty);
    }

    public bool IsValidPosition(int k)
    {
        if (k >= 0 && k <= Size) return true;
        return false;
    }
}