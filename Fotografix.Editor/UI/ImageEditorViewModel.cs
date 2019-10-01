using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fotografix.Editor.UI
{
    public sealed class ImageEditorViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Image image;
        private Adjustment selectedAdjustment;
        private BlendModeListItem selectedBlendMode;

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

        public bool AdjustmentPropertiesVisible => selectedAdjustment != null;

        public Adjustment SelectedAdjustment
        {
            get
            {
                return selectedAdjustment;
            }

            set
            {
                if (SetValue(ref selectedAdjustment, value))
                {
                    SelectedBlendMode = BlendModes[selectedAdjustment.BlendMode];
                    RaisePropertyChanged(nameof(AdjustmentPropertiesVisible));
                }
            }
        }

        public BlendModeList BlendModes { get; } = BlendModeList.Create();

        public BlendModeListItem SelectedBlendMode
        {
            get
            {
                return selectedBlendMode;
            }

            set
            {
                if (SetValue(ref selectedBlendMode, value))
                {
                    if (selectedAdjustment != null)
                    {
                        selectedAdjustment.BlendMode = selectedBlendMode.BlendMode;
                    }
                }
            }
        }

        public int SelectedBlendModeIndex
        {
            get
            {
                return selectedAdjustment == null ? 0 : (int)selectedAdjustment.BlendMode;
            }

            set
            {
                if (selectedAdjustment != null)
                {
                    selectedAdjustment.BlendMode = (BlendMode)value;
                }
            }
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            image.Draw(drawingSession);
        }

        public void AddAdjustment(Adjustment adjustment)
        {
            image.AddAdjustment(adjustment);
            this.SelectedAdjustment = image.Adjustments.Last();
        }
    }
}
