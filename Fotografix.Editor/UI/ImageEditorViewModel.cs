using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Fotografix.Editor.UI
{
    public sealed class ImageEditorViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Image image;
        private Adjustment selectedAdjustment;
        private BlendModeListItem selectedBlendMode;
        private DelegateCommand deleteAdjustmentCommand;

        public ImageEditorViewModel(Image image)
        {
            this.image = image;
            this.deleteAdjustmentCommand = new DelegateCommand(DeleteAdjustment, () => CanDeleteAdjustment);
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
                    if (selectedAdjustment != null)
                    {
                        SelectedBlendMode = BlendModes[selectedAdjustment.BlendMode];
                    }

                    RaisePropertyChanged(nameof(AdjustmentPropertiesVisible));
                    deleteAdjustmentCommand.RaiseCanExecuteChanged();
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

        public ICommand DeleteAdjustmentCommand => deleteAdjustmentCommand;

        private bool CanDeleteAdjustment => selectedAdjustment != null;

        private void DeleteAdjustment()
        {
            image.DeleteAdjustment(selectedAdjustment);
            this.SelectedAdjustment = image.Adjustments.LastOrDefault();
        }
    }
}
