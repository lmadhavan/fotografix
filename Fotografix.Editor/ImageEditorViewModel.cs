using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace Fotografix.Editor
{
    public sealed class ImageEditorViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Image image;
        private Adjustment selectedAdjustment;

        public ImageEditorViewModel(Image image)
        {
            this.image = image;
            this.BlendModes = GetBlendModeNames();
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
                    RaisePropertyChanged(nameof(SelectedBlendModeIndex));
                    RaisePropertyChanged(nameof(AdjustmentPropertiesVisible));
                }
            }
        }

        public IList<string> BlendModes { get; }

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

        private IList<string> GetBlendModeNames()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse("Fotografix.Editor/Resources/BlendMode");
            var enumNames = Enum.GetNames(typeof(BlendMode));
            return enumNames.Select(resourceLoader.GetString).ToList();
        }
    }
}
