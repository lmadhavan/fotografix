using Fotografix.Composition;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Fotografix.UI
{
    public sealed class ImageEditorViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Image image;
        private Layer selectedLayer;
        private BlendModeListItem selectedBlendMode;
        private DelegateCommand deleteLayerCommand;

        public ImageEditorViewModel(Image image)
        {
            this.image = image;
            this.selectedLayer = image.Layers[0];
            this.deleteLayerCommand = new DelegateCommand(DeleteLayer, () => CanDeleteLayer);
        }

        public void Dispose()
        {
            image.Dispose();
        }

        public event EventHandler Invalidated
        {
            add { image.Invalidated += value; }
            remove { image.Invalidated -= value; }
        }

        public int Width => image.Width;
        public int Height => image.Height;

        public ReadOnlyObservableCollection<Layer> Layers => image.Layers;

        public Layer SelectedLayer
        {
            get
            {
                return selectedLayer;
            }

            set
            {
                if (SetValue(ref selectedLayer, value))
                {
                    if (selectedLayer != null)
                    {
                        SelectedBlendMode = BlendModes[selectedLayer.BlendMode];
                    }

                    deleteLayerCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(IsBlendModeEnabled));
                }
            }
        }

        public bool IsBlendModeEnabled => selectedLayer != image.Layers[0];

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
                    if (selectedLayer != null)
                    {
                        selectedLayer.BlendMode = selectedBlendMode.BlendMode;
                    }
                }
            }
        }

        public int SelectedBlendModeIndex
        {
            get
            {
                return selectedLayer == null ? 0 : (int)selectedLayer.BlendMode;
            }

            set
            {
                if (selectedLayer != null)
                {
                    selectedLayer.BlendMode = (BlendMode)value;
                }
            }
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            image.Draw(drawingSession);
        }

        public void AddLayer(Layer layer)
        {
            image.AddLayer(layer);
            this.SelectedLayer = layer;
        }

        public ICommand DeleteLayerCommand => deleteLayerCommand;

        private bool CanDeleteLayer => selectedLayer != image.Layers[0];

        private void DeleteLayer()
        {
            Layer layer = selectedLayer;
            image.DeleteLayer(layer);
            layer.Dispose();
            this.SelectedLayer = image.Layers.Last();
        }
    }
}
