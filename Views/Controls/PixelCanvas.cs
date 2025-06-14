using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading; // For Dispatcher
using PixelWallEApp.Models; // For CanvasState
using System;
using System.Globalization;

namespace PixelWallEApp.Views.Controls
{
    public class PixelCanvas : Control
    {
        // DependencyProperty to bind the CanvasState from the ViewModel
        public static readonly StyledProperty<CanvasState?> CanvasStateProperty =
            AvaloniaProperty.Register<PixelCanvas, CanvasState?>(nameof(CanvasState));

        public CanvasState? CanvasState
        {
            get => GetValue(CanvasStateProperty);
            set => SetValue(CanvasStateProperty, value);
        }

        static PixelCanvas()
        {
            // Re-render whenever the CanvasState property changes
            AffectsRender<PixelCanvas>(CanvasStateProperty);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == CanvasStateProperty)
            {
                // If we were previously subscribed to an old CanvasState, unsubscribe
                if (change.OldValue is CanvasState oldState)
                {
                    oldState.PropertyChanged -= OnCanvasStateChanged;
                }

                // Subscribe to changes in the new CanvasState (like Resize)
                if (change.NewValue is CanvasState newState)
                {
                    newState.PropertyChanged += OnCanvasStateChanged;
                }
                // Trigger a redraw when the state object itself changes
                InvalidateVisual();
            }
        }

        // This method is called when a property *within* the CanvasState changes (e.g., Size)
        private void OnCanvasStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // We only care about changes that affect the visual representation (Size or the Pixels array itself)
            if (e.PropertyName == nameof(Models.CanvasState.Size) || e.PropertyName == nameof(Models.CanvasState.Pixels))
            {
                // Ensure redraw happens on the UI thread
                Dispatcher.UIThread.InvokeAsync(InvalidateVisual);
            }
        }


        // The core drawing method
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var state = CanvasState;
            if (state == null || state.Size == 0)
            {
                // Draw placeholder if no state or zero size
                // --- CORRECCIÓN AQUÍ ---
                var placeholderText = new FormattedText(
    "No Canvas Data",               // 1. text (string)
    CultureInfo.CurrentUICulture,   // 2. culture (CultureInfo)
    FlowDirection.LeftToRight,      // 3. flowDirection (FlowDirection)
    Typeface.Default,               // 4. typeface (Typeface)
    12.0,                           // 5. emSize (double)
    Brushes.Gray                    // 6. foreground (IBrush?) - ¡AQUÍ ESTÁ EL COLOR!
);
                // --- CORRECCIÓN AQUÍ ---
                // Llama a la sobrecarga que toma (FormattedText, Point)
                context.DrawText(
                    placeholderText,     // Argumento 1: El objeto FormattedText
                    new Point(10, 10)    // Argumento 2: El punto de origen
                );
                // --- FIN DE LA CORRECCIÓN ---
                return;
            }

            // Calculate pixel size based on control dimensions
            double pixelWidth = Bounds.Width / state.Size;
            double pixelHeight = Bounds.Height / state.Size;

            var pixels = state.Pixels;

            for (int y = 0; y < state.Size; y++)
            {
                for (int x = 0; x < state.Size; x++)
                {
                    var color = pixels[x, y];
                    // Optimization: Don't draw if transparent or default white background?
                    // if (color == Colors.White) continue; // Optional speedup

                    var rect = new Rect(x * pixelWidth, y * pixelHeight, pixelWidth, pixelHeight);
                    context.FillRectangle(new SolidColorBrush(color), rect);

                    // Optional: Draw grid lines if pixels are large enough
                    if (pixelWidth > 3 && pixelHeight > 3)
                    {
                        context.DrawRectangle(new Pen(Brushes.LightGray, 0.5), rect);
                    }
                }
            }

            // Optional: Draw Wall-E's current position marker?
            if (state != null && App.MainWindowViewModel?.WallE?.IsSpawned == true) // Check if VM and WallE exist
            {
                var wallE = App.MainWindowViewModel.WallE;
                var wallERect = new Rect(wallE.X * pixelWidth, wallE.Y * pixelHeight, pixelWidth, pixelHeight);
                context.DrawRectangle(new Pen(Brushes.Red, Math.Max(1, pixelWidth / 10.0)), wallERect); // Draw red outline
            }
        }

        // Request redraw when the control is resized
        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            InvalidateVisual();
        }
    }
}