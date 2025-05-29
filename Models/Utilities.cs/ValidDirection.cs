using Avalonia; // Para Point
using PixelWallEApp.Models;
using PixelWallEApp.ViewModels; // Para MainWindowViewModel
using System.Diagnostics; // Para Debug.WriteLine
using PixelWallEApp.Models.Commands;

namespace PixelWallEApp.Commands
{
    public static class IsDirectionValid
    {
        public static bool DirectionValid(int dirValue)
        {
            return dirValue >= -1 && dirValue <= 1;
        }
    }
}