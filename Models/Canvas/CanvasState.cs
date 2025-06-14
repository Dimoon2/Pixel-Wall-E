// Models/PixelCanvas.cs
using Avalonia.Media;
using System;
using System.Diagnostics.CodeAnalysis; // Needed for MemberNotNull attribute

namespace PixelWallEApp.Models.Canvas;

public class CanvasState
{
    // The field that caused the warning
    private Color[,] _pixels; // Now it will be initialized in the constructor

    public int Size { get; private set; }

    public event EventHandler? CanvasInvalidated; // Event to notify when canvas changes

    // Constructor NOW initializes _pixels directly
    public CanvasState(int size)
    {
        // Validate dimensions right at the start
        if (size <= 0)
        {
            // Or handle differently, maybe default to a minimum size?
            throw new ArgumentOutOfRangeException(nameof(size) + "/" + nameof(size), "Canvas dimensions must be positive.");
        }

        Size = size;
        // **Initialize the array here**
        _pixels = new Color[Size, Size];

        // Clear to the initial state (white)
        Clear(Colors.White);
        // Note: Clear calls NotifyChanged, so we don't need it here explicitly
    }

    // Method to resize AND clear the canvas
    // It now assumes _pixels already exists but needs reallocation
    public void Resize(int newSize)
    {
        if (newSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newSize) + "/" + nameof(newSize), "Canvas dimensions must be positive.");
        }

        // Only reallocate and clear if dimensions actually change
        if (newSize != Size)
        {
            Size = newSize;
            _pixels = new Color[newSize, newSize]; // Reallocate
            Clear(Colors.White); // Clear the new array
        }
        // If dimensions are the same, do nothing or just clear?
        // Current behaviour: only acts on size change. If you want resize(same, same)
        // to clear, call Clear(Colors.White) outside the if block.
    }

    // Clear the canvas with a specific color
    public void Clear(Color color)
    {
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                _pixels[x, y] = color;
            }
        }
        NotifyChanged(); // Notify after clearing
    }

    // Set a single pixel color, handling bounds
    public bool SetPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < Size && y >= 0 && y < Size)
        {
            _pixels[x, y] = color;
            // We will call NotifyChanged() after the *entire* drawing operation (like DrawLine)
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
        // Consistent with SetPixel, out-of-bounds reads can return a default
        return Colors.Transparent; // Or throw? Transparent seems reasonable.
    }


    // Helper to get the raw pixel data (useful for rendering)
    public Color[,] GetPixelsData()
    {
        return _pixels;
    }

    // Call this after a drawing operation completes
    public void NotifyChanged()
    {
        CanvasInvalidated?.Invoke(this, EventArgs.Empty);
    }

    public bool IsValidPosition(int k)
    {
        if (k > 0 && k < Size) return true;
        return false;
    }
}