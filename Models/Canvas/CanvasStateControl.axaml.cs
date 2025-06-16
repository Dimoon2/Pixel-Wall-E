using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging; // Needed for Bitmap
using Avalonia.Media.TextFormatting; // Needed for FormattedText
using Avalonia.Platform; // Needed for IAssetLoader
using Avalonia.Threading;
using PixelWallEApp.Models.Canvas; // Your models namespace
using System;
using System.Diagnostics; // For Debug output
using System.Globalization; // Needed for CultureInfo

namespace PixelWallEApp.Models.Canvas;

public partial class CanvasStateControl : UserControl
{
    // --- Canvas Data Property ---
    public static readonly StyledProperty<CanvasState?> CanvasDataProperty =
        AvaloniaProperty.Register<CanvasStateControl, CanvasState?>(nameof(CanvasData));

    public CanvasState? CanvasData
    {
        get => GetValue(CanvasDataProperty);
        set => SetValue(CanvasDataProperty, value);
    }

    // --- Axis Styling Properties ---
    public static readonly StyledProperty<double> AxisThicknessProperty =
        AvaloniaProperty.Register<CanvasStateControl, double>(nameof(AxisThickness), defaultValue: 25.0);

    public double AxisThickness
    {
        get => GetValue(AxisThicknessProperty);
        set => SetValue(AxisThicknessProperty, value);
    }

    public static readonly StyledProperty<IBrush> AxisBackgroundProperty =
        AvaloniaProperty.Register<CanvasStateControl, IBrush>(nameof(AxisBackground), defaultValue: Brushes.LightGray);

    public IBrush AxisBackground
    {
        get => GetValue(AxisBackgroundProperty);
        set => SetValue(AxisBackgroundProperty, value);
    }

    public static readonly StyledProperty<IBrush> AxisForegroundProperty =
        AvaloniaProperty.Register<CanvasStateControl, IBrush>(nameof(AxisForeground), defaultValue: Brushes.Black);

    public IBrush AxisForeground
    {
        get => GetValue(AxisForegroundProperty);
        set => SetValue(AxisForegroundProperty, value);
    }

    // --- Wall-E Position Properties ---
    public static readonly StyledProperty<int> WallEXProperty =
        AvaloniaProperty.Register<CanvasStateControl, int>(nameof(WallEX), defaultValue: -1);

    public int WallEX
    {
        get => GetValue(WallEXProperty);
        set => SetValue(WallEXProperty, value);
    }

    public static readonly StyledProperty<int> WallEYProperty =
        AvaloniaProperty.Register<CanvasStateControl, int>(nameof(WallEY), defaultValue: -1);

    public int WallEY
    {
        get => GetValue(WallEYProperty);
        set => SetValue(WallEYProperty, value);
    }

    // --- Wall-E Icon Resource ---
    private Bitmap? _wallEIcon;
    private bool _iconLoadAttempted = false;

    // --- Static Constructor for Property Change Notifications ---
    static CanvasStateControl()
    {
        // Re-render when any of these properties change
        AffectsRender<CanvasStateControl>(
            CanvasDataProperty,
            AxisThicknessProperty, AxisBackgroundProperty, AxisForegroundProperty,
            WallEXProperty, WallEYProperty
        );
    }

    // --- Constructor ---
    public CanvasStateControl()
    {
        InitializeComponent();
        // Icon loading moved to OnAttachedToVisualTree
    }

    // --- Lifecycle Event for Resource Loading ---
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        LoadWallEIcon(); // Attempt to load icon when control is ready
    }

    // --- Load Wall-E Icon Method ---
    private void LoadWallEIcon()
{
    if (_wallEIcon != null || _iconLoadAttempted) return; // Already loaded or load attempted

    _iconLoadAttempted = true;
    try
    {
        // Make sure your assembly name and path are correct!
        string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "PixelWallE"; // Fallback name
        string uriString = $"avares://{assemblyName}/Assets/pets.png";
        Uri iconUri = new Uri(uriString);

        // --- Use static AssetLoader instead of AvaloniaLocator ---
        if (AssetLoader.Exists(iconUri)) // Check existence using static method
        {
            using (var stream = AssetLoader.Open(iconUri)) // Open using static method
            {
                _wallEIcon = new Bitmap(stream);
                Debug.WriteLine("Wall-E icon loaded successfully.");
            }
        }
        else
        {
            Debug.WriteLine($"Error: Wall-E icon asset not found at {iconUri}");
        }
        // --- End of change ---
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error loading Wall-E icon: {ex.Message}");
        _wallEIcon = null; // Ensure it's null if loading failed
    }
    InvalidateVisual(); // Request a redraw in case loading finished after initial render
}

    // --- Property Changed Handler (for CanvasData invalidation subscription) ---
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CanvasDataProperty)
        {
            // Unsubscribe from old canvas events, subscribe to new ones
            if (change.OldValue is CanvasState oldCanvas)
            {
                oldCanvas.CanvasInvalidated -= OnCanvasInvalidated;
            }
            if (change.NewValue is CanvasState newCanvas)
            {
                newCanvas.CanvasInvalidated += OnCanvasInvalidated;
            }
            // Trigger a render when the canvas reference changes
            InvalidateVisual();
        }
    }

    // --- Canvas Invalidation Handler ---
    private void OnCanvasInvalidated(object? sender, EventArgs e)
    {
        // Ensure the redraw happens on the UI thread
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

    // --- Main Rendering Method ---
    public override void Render(DrawingContext context)
    {
        base.Render(context); // Call base render first

        var canvas = CanvasData;
        double axisThickness = AxisThickness;
        var bounds = Bounds; // The total area of the control

        // Calculate the area available for the actual pixel grid
        var pixelAreaBounds = new Rect(
            bounds.Left + axisThickness, // Start after left axis
            bounds.Top + axisThickness,  // Start after top axis
            Math.Max(0, bounds.Width - axisThickness), // Adjust width
            Math.Max(0, bounds.Height - axisThickness) // Adjust height
        );

        // If no canvas or calculated pixel area is invalid, draw background and exit
        if (canvas == null || canvas.Size <= 0 || canvas.Size <= 0 || pixelAreaBounds.Width <= 0 || pixelAreaBounds.Height <= 0)
        {
            context.DrawRectangle(Background ?? Brushes.Transparent, null, bounds); // Draw control background
            DrawAxisBackgrounds(context, bounds, pixelAreaBounds, axisThickness); // Still draw axis backgrounds
            return;
        }

        // Calculate pixel size based on the *pixel area*
        double pixelWidth = pixelAreaBounds.Width / canvas.Size;
        double pixelHeight = pixelAreaBounds.Height / canvas.Size;

        // --- Draw Phase ---

        // Draw control background (if any)
        if (Background != null)
        {
            context.DrawRectangle(Background, null, bounds);
        }

        // Draw Axis Backgrounds
        DrawAxisBackgrounds(context, bounds, pixelAreaBounds, axisThickness);

        // Draw the Pixel Grid (within pixelAreaBounds)
        DrawPixelGrid(context, canvas, pixelAreaBounds, pixelWidth, pixelHeight);

        // Draw Grid Lines (optional, within pixelAreaBounds)
        DrawGridLines(context, canvas, pixelAreaBounds, pixelWidth, pixelHeight);

        // Draw Wall-E Icon (within pixelAreaBounds)
        DrawWallEIcon(context, canvas, pixelAreaBounds, pixelWidth, pixelHeight);

        // Draw Axis Ticks and Labels
        DrawAxes(context, canvas, bounds, pixelAreaBounds, pixelWidth, pixelHeight, axisThickness);
    }

    // --- Helper Methods ---

    private void DrawAxisBackgrounds(DrawingContext context, Rect bounds, Rect pixelAreaBounds, double axisThickness)
    {
        var background = AxisBackground;
        if (background == null || axisThickness <= 0) return;

        // Top axis background
        context.DrawRectangle(background, null, new Rect(bounds.Left, bounds.Top, bounds.Width, axisThickness));
        // Left axis background
        context.DrawRectangle(background, null, new Rect(bounds.Left, bounds.Top + axisThickness, axisThickness, Math.Max(0, bounds.Height - axisThickness)));
        // You could draw the corner square explicitly if desired:
        // context.DrawRectangle(background, null, new Rect(bounds.Left, bounds.Top, axisThickness, axisThickness));
    }

    private void DrawPixelGrid(DrawingContext context, CanvasState canvas, Rect pixelAreaBounds, double pixelWidth, double pixelHeight)
    {
        var pixels = canvas.GetPixelsData();

        for (int y = 0; y < canvas.Size; y++)
        {
            for (int x = 0; x < canvas.Size; x++)
            {
                Color color = pixels[x, y];
                if (color.A == 0) continue; // Skip fully transparent pixels

                var pixelBrush = new SolidColorBrush(color); // Create brush per pixel

                // Calculate position RELATIVE to the pixelAreaBounds top-left
                var rect = new Rect(
                    pixelAreaBounds.Left + x * pixelWidth,
                    pixelAreaBounds.Top + y * pixelHeight,
                    pixelWidth,
                    pixelHeight);

                context.DrawRectangle(pixelBrush, null, rect);
            }
        }
    }

    private void DrawGridLines(DrawingContext context, CanvasState canvas, Rect pixelAreaBounds, double pixelWidth, double pixelHeight)
    {
        // Only draw grid if pixels are reasonably large and grid visible
        if (pixelWidth < 3 || pixelHeight < 3) return;

        var gridPen = new Pen(Brushes.LightGray, 0.5); // Use a fixed light gray

        // Vertical lines
        for (int x = 0; x <= canvas.Size; x++)
        {
            double lineX = pixelAreaBounds.Left + x * pixelWidth;
            // Clip line drawing slightly within bounds for cleaner look
            context.DrawLine(gridPen, new Point(lineX, pixelAreaBounds.Top), new Point(lineX, pixelAreaBounds.Bottom));
        }
        // Horizontal lines
        for (int y = 0; y <= canvas.Size; y++)
        {
            double lineY = pixelAreaBounds.Top + y * pixelHeight;
            context.DrawLine(gridPen, new Point(pixelAreaBounds.Left, lineY), new Point(pixelAreaBounds.Right, lineY));
        }
    }

     private void DrawWallEIcon(DrawingContext context, CanvasState canvas, Rect pixelAreaBounds, double pixelWidth, double pixelHeight)
    {
        // Make sure icon is loaded and Wall-E position is valid
        if (_wallEIcon == null)
        {
            // Maybe attempt to load again? Be cautious about performance/loops.
            // if (!_iconLoadAttempted) LoadWallEIcon();
            return; // Skip drawing if icon isn't ready
        }

        int walleX = WallEX;
        int walleY = WallEY;

        // Check if Wall-E's position is within the canvas bounds
        if (walleX < 0 || walleX >= canvas.Size || walleY < 0 || walleY >= canvas.Size)
        {
            return; // Wall-E is not spawned or is outside the visible canvas
        }

        // Calculate the destination rectangle for the icon
        // It should cover the pixel at (walleX, walleY)
        var destRect = new Rect(
            pixelAreaBounds.Left + walleX * pixelWidth,
            pixelAreaBounds.Top + walleY * pixelHeight,
            pixelWidth,  // Draw the icon scaled to the pixel width
            pixelHeight  // Draw the icon scaled to the pixel height
        );

        // Draw the image onto the calculated rectangle
        // Source rectangle is the whole bitmap: new Rect(_wallEIcon.Size)
        context.DrawImage(_wallEIcon, new Rect(_wallEIcon.Size), destRect);
    }


    private void DrawAxes(DrawingContext context, CanvasState canvas, Rect bounds, Rect pixelAreaBounds, double pixelWidth, double pixelHeight, double axisThickness)
    {
        if (axisThickness <= 5) return; // Not enough space for readable axes

        var foreground = AxisForeground;
        if (foreground == null) foreground = Brushes.Black; // Fallback
        // var pen = new Pen(foreground, 1.0); // Pen no longer needed if not drawing ticks

        // --- Adaptive Sizing ---
        int xStep = 1;
        if (pixelWidth < 6) xStep = 10; else if (pixelWidth < 12) xStep = 5; else if (pixelWidth < 25) xStep = 2;

        int yStep = 1;
        if (pixelHeight < 6) yStep = 10; else if (pixelHeight < 12) yStep = 5; else if (pixelHeight < 25) yStep = 2;

        double fontSize = Math.Clamp(Math.Min(pixelWidth, pixelHeight) * 0.6, 7.0, 12.0);
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight);

        // --- Draw Top Axis (X coordinates) ---
        for (int x = 0; x < canvas.Size; x += xStep)
        {
            double labelCenterX = pixelAreaBounds.Left + (x + 0.5) * pixelWidth; // Center of the pixel column

            // --- REMOVED TICK DRAWING ---
            // double tickStartY = bounds.Top + axisThickness * 0.7;
            // double tickEndY = pixelAreaBounds.Top;
            // context.DrawLine(pen, new Point(labelCenterX, tickStartY), new Point(labelCenterX, tickEndY));
            // --- END REMOVAL ---

            var labelText = new FormattedText(
                x.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                typeface, fontSize, foreground
            );

            // Adjust text position to center within the axis thickness area
            double textX = labelCenterX - (labelText.Width / 2.0);
            double textY = bounds.Top + (axisThickness - labelText.Height) / 2.0; // Center vertically in axis area

            if (textX < bounds.Left) textX = bounds.Left;
            if (textX + labelText.Width > bounds.Right) continue;

            context.DrawText(labelText, new Point(textX, Math.Max(bounds.Top, textY)));
        }

        // --- Draw Left Axis (Y coordinates) ---
        for (int y = 0; y < canvas.Size; y += yStep)
        {
            double labelCenterY = pixelAreaBounds.Top + (y + 0.5) * pixelHeight; // Center of the pixel row


            var labelText = new FormattedText(
                y.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                typeface, fontSize, foreground
            );

            // Adjust text position to center within the axis thickness area
            double textX = bounds.Left + (axisThickness - labelText.Width) / 2.0; // Center horizontally in axis area
            double textY = labelCenterY - (labelText.Height / 2.0); // Center vertically around the middle of the row

            if (textY < pixelAreaBounds.Top) textY = pixelAreaBounds.Top;
            if (textY + labelText.Height > bounds.Bottom) continue;
            if (textX < bounds.Left) textX = bounds.Left; // Prevent drawing off-left
            if (textX + labelText.Width > pixelAreaBounds.Left) continue; // Prevent drawing into pixel area

            context.DrawText(labelText, new Point(Math.Max(bounds.Left, textX), textY));
        }
    }
}