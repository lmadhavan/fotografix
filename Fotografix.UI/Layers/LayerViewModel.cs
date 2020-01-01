using Fotografix.UI.Adjustments;
using Fotografix.UI.BlendModes;
using System.ComponentModel;

namespace Fotografix.UI.Layers
{
    public sealed class LayerViewModel : PropertyEditorViewModelBase<Layer>
    {
        private static readonly BlendModeList BlendModeList = BlendModeList.Create();

        private IAdjustmentViewModel adjustmentViewModel;

        public LayerViewModel(Layer layer, ICommandService commandService) : base(layer, commandService)
        {
            CreateAdjustmentViewModel();
        }

        public string Name
        {
            get => Target.Name;
            set => SetTargetProperty(value);
        }

        public BlendModeList AvailableBlendModes => BlendModeList;

        public BlendModeListItem BlendMode
        {
            get => BlendModeList[Target.BlendMode];
            set => SetTargetProperty(value.BlendMode);
        }

        public float Opacity
        {
            get => Target.Opacity;
            set => SetTargetProperty(value);
        }

        public IAdjustmentViewModel AdjustmentViewModel
        {
            get
            {
                return adjustmentViewModel;
            }

            private set
            {
                var oldValue = adjustmentViewModel;

                if (SetProperty(ref adjustmentViewModel, value))
                {
                    oldValue?.Dispose();
                }
            }
        }

        protected override void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AdjustmentLayer.Adjustment))
            {
                CreateAdjustmentViewModel();
            }

            base.OnTargetPropertyChanged(sender, e);
        }

        private void CreateAdjustmentViewModel()
        {
            Target.Accept(new AdjustmentViewModelFactoryVisitor(this));
        }

        private sealed class AdjustmentViewModelFactoryVisitor : LayerVisitor
        {
            private readonly LayerViewModel layerViewModel;

            public AdjustmentViewModelFactoryVisitor(LayerViewModel layerViewModel)
            {
                this.layerViewModel = layerViewModel;
            }

            public override void Visit(AdjustmentLayer layer)
            {
                layerViewModel.AdjustmentViewModel = AdjustmentViewModelFactory.CreateAdjustmentViewModel(layer.Adjustment, layerViewModel.CommandService);
            }
        }
    }
}
