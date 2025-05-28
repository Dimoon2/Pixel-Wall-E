using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using PixelWallEApp.Models; // Para LineModel


namespace PixelWallEApp.Controls
{
    public class PixelCanvas : Control
    {
        // ... (propiedades ItemsSource y LogicalSize sin cambios) ...
        public static readonly StyledProperty<IEnumerable<LineModel>?> ItemsSourceProperty =
            AvaloniaProperty.Register<PixelCanvas, IEnumerable<LineModel>?>(nameof(ItemsSource));

        public IEnumerable<LineModel>? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<int> LogicalSizeProperty =
            AvaloniaProperty.Register<PixelCanvas, int>(nameof(LogicalSize), defaultValue: 50);

        public int LogicalSize
        {
            get => GetValue(LogicalSizeProperty);
            set => SetValue(LogicalSizeProperty, value);
        }
        
        // Nueva propiedad para el color de la cuadrícula
        public static readonly StyledProperty<IBrush> GridBrushProperty =
            AvaloniaProperty.Register<PixelCanvas, IBrush>(nameof(GridBrush), defaultValue: Brushes.LightGray);

        public IBrush GridBrush
        {
            get => GetValue(GridBrushProperty);
            set => SetValue(GridBrushProperty, value);
        }

        // Nueva propiedad para la visibilidad de la cuadrícula
        public static readonly StyledProperty<bool> ShowGridProperty =
            AvaloniaProperty.Register<PixelCanvas, bool>(nameof(ShowGrid), defaultValue: true);

        public bool ShowGrid
        {
            get => GetValue(ShowGridProperty);
            set => SetValue(ShowGridProperty, value);
        }


        static PixelCanvas()
        {
            // Si cambia ItemsSource, el tamaño del control (Bounds), el tamaño lógico,
            // o las propiedades de la cuadrícula, redibujar.
            AffectsRender<PixelCanvas>(
                ItemsSourceProperty, 
                BoundsProperty, 
                LogicalSizeProperty, 
                GridBrushProperty, 
                ShowGridProperty 
            );
            
            ItemsSourceProperty.Changed.AddClassHandler<PixelCanvas>((s, e) => s.OnItemsSourceChanged(e));
        }
        
        public PixelCanvas()
        {
            ClipToBounds = true;
        }

        // ... (OnItemsSourceChanged, OnItemsSourceCollectionChanged, OnDetachedFromVisualTree sin cambios) ...
        private void OnItemsSourceChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnItemsSourceCollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnItemsSourceCollectionChanged;
            }
            InvalidateVisual();
        }

        private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();
        }
        
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            if (ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= OnItemsSourceCollectionChanged;
            }
        }


        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
            var actualWidth = Bounds.Width;
            var actualHeight = Bounds.Height;
            var currentLogicalSize = LogicalSize;

            if (actualWidth <= 0 || actualHeight <= 0 || currentLogicalSize <= 0)
            {
                return; 
            }

            double pixelDisplaySize = Math.Min(actualWidth / currentLogicalSize, actualHeight / currentLogicalSize);

            if (pixelDisplaySize <= 0) return; // Evitar división por cero o tamaños negativos si algo va mal

            double renderedLogicalWidth = currentLogicalSize * pixelDisplaySize;
            double renderedLogicalHeight = currentLogicalSize * pixelDisplaySize;

            double offsetX = (actualWidth - renderedLogicalWidth) / 2.0;
            double offsetY = (actualHeight - renderedLogicalHeight) / 2.0;

            // 1. Dibujar fondo blanco del área lógica
            context.FillRectangle(Brushes.White, new Rect(offsetX, offsetY, renderedLogicalWidth, renderedLogicalHeight));

            // 2. Dibujar la cuadrícula (Grid) si está habilitada y los píxeles son suficientemente visibles
            if (ShowGrid && pixelDisplaySize > 1) // Umbral para que la rejilla no sea demasiado densa
            {
                Pen gridPen = new Pen(GridBrush, 0.5); // Grosor fijo delgado para la rejilla

                // Líneas Verticales
                for (int i = 0; i <= currentLogicalSize; i++)
                {
                    double x = offsetX + (i * pixelDisplaySize);
                    // Asegurarse de que las líneas de la rejilla no se dibujen fuera de los bounds del control
                    // aunque el ClipToBounds debería ayudar.
                    context.DrawLine(gridPen, 
                                     new Point(x, offsetY), 
                                     new Point(x, offsetY + renderedLogicalHeight));
                }
                // Líneas Horizontales
                for (int i = 0; i <= currentLogicalSize; i++)
                {
                    double y = offsetY + (i * pixelDisplaySize);
                    context.DrawLine(gridPen, 
                                     new Point(offsetX, y), 
                                     new Point(offsetX + renderedLogicalWidth, y));
                }
            }

            // 3. Dibujar las líneas del usuario (ItemsSource)
            var lines = ItemsSource;
            if (lines != null)
            {
                foreach (var line in lines)
                {
                    if (line.Brush != Brushes.Transparent)
                    {
                        Point startScreen = new Point(
                            offsetX + line.Start.X * pixelDisplaySize + (pixelDisplaySize / 2.0),
                            offsetY + line.Start.Y * pixelDisplaySize + (pixelDisplaySize / 2.0)
                        );
                        Point endScreen = new Point(
                            offsetX + line.End.X * pixelDisplaySize + (pixelDisplaySize / 2.0),
                            offsetY + line.End.Y * pixelDisplaySize + (pixelDisplaySize / 2.0)
                        );

                        double scaledThickness = Math.Max(1.0, line.Thickness * pixelDisplaySize * 0.9); // Ajuste ligero para que no sea tan grueso
                         if (line.Thickness == 1 && pixelDisplaySize > 0)
                         {
                             scaledThickness = pixelDisplaySize; 
                         }


                        var pen = new Pen(line.Brush, scaledThickness,
                                          lineCap: PenLineCap.Square, 
                                          lineJoin: PenLineJoin.Miter); // Miter o Bevel

                        context.DrawLine(pen, startScreen, endScreen);
                    }
                }
            }
        }
    }
}