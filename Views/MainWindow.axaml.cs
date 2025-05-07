using Avalonia.Controls;
using PixelWallEApp.ViewModels; // Required for ViewModel type

namespace PixelWallEApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Find the ViewModel instance (assuming it's set in App.axaml.cs or Program.cs)
            // And give it a reference to the canvas control.
            if (this.DataContext is MainWindowViewModel vm)
            {
                // Find the canvas control by its Name property in XAML
                var canvasControl = this.FindControl<Controls.PixelCanvas>("PixelCanvasDisplay");
                vm.CanvasControl = canvasControl;
            }
        }

    }
}