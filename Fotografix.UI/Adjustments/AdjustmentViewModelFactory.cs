using Fotografix.Adjustments;

namespace Fotografix.UI.Adjustments
{
    public static class AdjustmentViewModelFactory
    {
        public static IAdjustmentViewModel CreateAdjustmentViewModel(Adjustment adjustment, ICommandService commandService)
        {
            FactoryVisitor visitor = new FactoryVisitor(commandService);
            adjustment.Accept(visitor);
            return visitor.ViewModel;
        }

        private sealed class FactoryVisitor : AdjustmentVisitor
        {
            private readonly ICommandService commandService;

            public FactoryVisitor(ICommandService commandService)
            {
                this.commandService = commandService;
            }

            public IAdjustmentViewModel ViewModel { get; private set; }

            public override void Visit(BrightnessContrastAdjustment adjustment)
            {
                this.ViewModel = new BrightnessContrastAdjustmentViewModel(adjustment, commandService);
            }

            public override void Visit(GradientMapAdjustment adjustment)
            {
                this.ViewModel = new GradientMapAdjustmentViewModel(adjustment, commandService);
            }

            public override void Visit(HueSaturationAdjustment adjustment)
            {
                this.ViewModel = new HueSaturationAdjustmentViewModel(adjustment, commandService);
            }
        }
    }
}
