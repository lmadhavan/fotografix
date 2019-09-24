using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fotografix.Editor
{
    public sealed class ImageEditorViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Image image;
        private Adjustment selectedAdjustment;

        public ImageEditorViewModel(Image image)
        {
            this.image = image;
        }

        public void Dispose()
        {
            foreach (Adjustment adjustment in image.Adjustments)
            {
                adjustment.Dispose();
            }

            image.Dispose();
        }

        public event EventHandler Invalidated
        {
            add { image.Invalidated += value; }
            remove { image.Invalidated -= value; }
        }

        public int Width => image.Width;
        public int Height => image.Height;

        public ReadOnlyObservableCollection<Adjustment> Adjustments => image.Adjustments;

        public Adjustment SelectedAdjustment
        {
            get
            {
                return selectedAdjustment;
            }

            set
            {
                SetValue(ref selectedAdjustment, value);
            }
        }

        public IList<string> BlendModes { get; } = Enum.GetNames(typeof(BlendMode)).ToList();
        public int SelectedBlendModeIndex { get; set; }
        public BlendMode SelectedBlendMode => (BlendMode)SelectedBlendModeIndex;

        public void Draw(CanvasDrawingSession drawingSession)
        {
            image.Draw(drawingSession);
        }

        public void AddBlackAndWhiteAdjustment()
        {
            image.AddAdjustment(new BlackAndWhiteAdjustment(SelectedBlendMode));
        }

        public void AddShadowsHighlightsAdjustment()
        {
            image.AddAdjustment(new ShadowsHighlightsAdjustment());
        }
    }
}
