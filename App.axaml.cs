using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PixelWallEApp.ViewModels;
using PixelWallEApp.Views;

namespace PixelWallEApp
{
    public partial class App : Application
    {
        // Static reference to the ViewModel for easy access (e.g., from Canvas)
        // Consider dependency injection for larger apps.
        public static MainWindowViewModel? MainWindowViewModel { get; set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // En App.axaml.cs
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Asegúrate que creas el ViewModel ANTES de la ventana
                MainWindowViewModel = new MainWindowViewModel();

                // Asegúrate que asignas el ViewModel al DataContext
                desktop.MainWindow = new MainWindow
                {
                    DataContext = MainWindowViewModel, // Esta línea es crucial
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}