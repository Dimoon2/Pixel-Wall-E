using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
// Ensure this using is present and correct:
using PixelWallEApp.Models;
using System;
using System.Globalization;

namespace PixelWallEApp.Views.Controls
{
    public class PixelCanvas : Control
    {
        // --- DEFINE THE STATIC PROPERTY FIRST ---
        public static readonly StyledProperty<PixelWallEApp.Models.CanvasState?> CanvasStateProperty =
            AvaloniaProperty.Register<PixelCanvas, PixelWallEApp.Models.CanvasState?>(nameof(CanvasState));
            // ^-- Try fully qualifying the type here

        // --- STATIC CONSTRUCTOR (Uses the property) ---
        static PixelCanvas()
        {
            // Re-render whenever the CanvasState property changes
            // CS0103 would happen here if CanvasStateProperty wasn't defined above
            AffectsRender<PixelCanvas>(CanvasStateProperty);
        }

        // --- INSTANCE PROPERTY WRAPPER (Uses the property) ---
        public PixelWallEApp.Models.CanvasState? CanvasState // Use fully qualified type here too? (Optional usually)
        {
            // CS0103 would happen here if CanvasStateProperty wasn't defined above
            get => GetValue(CanvasStateProperty);
            set => SetValue(CanvasStateProperty, value);
        }

        // ... rest of your class (OnPropertyChanged, Render, etc.) ...

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == CanvasStateProperty) // Usage is okay here if defined above
            {
                // ... (unsubscribe/subscribe logic) ...
                InvalidateVisual();
            }
        }

         private void OnCanvasStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
         {
             if (e.PropertyName == nameof(Models.CanvasState.Size) || e.PropertyName == nameof(Models.CanvasState.Pixels))
             {
                  Dispatcher.UIThread.InvokeAsync(InvalidateVisual);
             }
         }

         protected override void OnSizeChanged(SizeChangedEventArgs e)
         {
             base.OnSizeChanged(e);
             InvalidateVisual();
         }
    }
}