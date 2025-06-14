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

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                 // Create the ViewModel first
                 MainWindowViewModel = new MainWindowViewModel();

                 // Then create the Window and assign the ViewModel to its DataContext
                 desktop.MainWindow = new MainWindow
                 {
                     DataContext = MainWindowViewModel,
                 };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}