// Models/WallEStateModel.cs
using Avalonia;
using Avalonia.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PixelWallEApp.Models
{
    public class WallEStateModel : INotifyPropertyChanged
    {
        private Point _currentPosition;
        public Point CurrentPosition
        {
            get => _currentPosition;
            set => SetField(ref _currentPosition, value);
        }

        private IBrush _brushColor; // Campo subyacente
        public IBrush BrushColor    // Propiedad pública
        {
            get => _brushColor;
            set => SetField(ref _brushColor, value);
        }

        private int _penThickness;
        public int PenThickness
        {
            get => _penThickness;
            set => SetField(ref _penThickness, value);
        }

        public WallEStateModel()
        {
            // Inicializar los campos directamente o a través de las propiedades
            // Si se usan propiedades, el compilador podría no verlo directamente.
            // La inicialización directa de campos es más clara para el análisis de nulabilidad.
            _currentPosition = new Point(0, 0);
            _brushColor = Brushes.Transparent; // Inicialización directa del campo
            _penThickness = 1;

            // O, si prefieres usar las propiedades (que también es válido y notifica cambios si es necesario al inicio):
            // CurrentPosition = new Point(0, 0);
            // BrushColor = Brushes.Transparent; // Esto llamará a SetField
            // PenThickness = 1;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}