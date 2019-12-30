using Fotografix.Adjustments;
using Fotografix.Editor.Commands;
using Fotografix.UI.BlendModes;
using System;
using System.ComponentModel;

namespace Fotografix.UI.Layers
{
    public sealed class LayerViewModel : NotifyPropertyChangedBase, ILayerVisitor, IDisposable
    {
        private static readonly BlendModeList BlendModeList = BlendModeList.Create();

        private readonly Layer layer;
        private readonly ICommandService commandService;

        public LayerViewModel(Layer layer, ICommandService commandService)
        {
            this.layer = layer;
            layer.Accept(this);
            layer.PropertyChanged += OnLayerPropertyChanged;
            
            this.commandService = commandService;
        }

        public void Dispose()
        {
            layer.PropertyChanged -= OnLayerPropertyChanged;
        }

        public string Name
        {
            get => layer.Name;
            set => commandService.Execute(new ChangePropertyCommand(layer, nameof(layer.Name), value));
        }

        public BlendModeList AvailableBlendModes => BlendModeList;

        public BlendModeListItem BlendMode
        {
            get => BlendModeList[layer.BlendMode];
            set => commandService.Execute(new ChangePropertyCommand(layer, nameof(layer.BlendMode), value.BlendMode));
        }

        public float Opacity
        {
            get => layer.Opacity;
            set => commandService.Execute(new ChangePropertyCommand(layer, nameof(layer.Opacity), value));
        }

        public Adjustment Adjustment { get; private set; }

        private void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(layer.Name):
                case nameof(layer.BlendMode):
                case nameof(layer.Opacity):
                    RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }

        void ILayerVisitor.Visit(AdjustmentLayer layer)
        {
            this.Adjustment = layer.Adjustment;
        }

        void ILayerVisitor.Visit(BitmapLayer layer)
        {
        }
    }
}
