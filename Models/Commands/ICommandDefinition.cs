using Avalonia.Controls;
using PixelWallEApp.Models; // For WallEState, CanvasState

namespace PixelWallEApp.Models.Commands
{
    public interface ICommandDefinition
    {
       void Execute(ViewModels.MainWindowViewModel viewModel, params object[] parameters);
        string CommandName { get; } // Para identificar el comando
    }
}