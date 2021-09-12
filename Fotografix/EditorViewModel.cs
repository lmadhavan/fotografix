using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;
using Windows.Foundation;

namespace Fotografix
{
    public sealed class EditorViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly PhotoEditor editor;
        private readonly PhotoAdjustment adjustment;

        public EditorViewModel(PhotoEditor editor)
        {
            this.editor = editor;
            this.adjustment = editor.Adjustment;
            adjustment.PropertyChanged += Adjustment_PropertyChanged;
        }

        public void Dispose()
        {
            adjustment.PropertyChanged -= Adjustment_PropertyChanged;
            editor.Dispose();
        }

        public PhotoAdjustment Adjustment => adjustment;
        public Size Size => editor.Size;
        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            editor.Draw(ds);
        }

        private void Adjustment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}