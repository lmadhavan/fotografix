using Fotografix.Adjustments;

namespace Fotografix.History.Adjustments
{
    public sealed class ChangeTrackingAdjustmentFactory : IAdjustmentFactory
    {
        private readonly IAdjustmentFactory factory;
        private readonly IChangeTracker changeTracker;

        public ChangeTrackingAdjustmentFactory(IAdjustmentFactory factory, IChangeTracker changeTracker)
        {
            this.factory = factory;
            this.changeTracker = changeTracker;
        }

        public IBlackAndWhiteAdjustment CreateBlackAndWhiteAdjustment()
        {
            return factory.CreateBlackAndWhiteAdjustment();
        }

        public IBrightnessContrastAdjustment CreateBrightnessContrastAdjustment()
        {
            return new ChangeTrackingBrightnessContrastAdjustment(factory.CreateBrightnessContrastAdjustment(), changeTracker);
        }

        public IGradientMapAdjustment CreateGradientMapAdjustment()
        {
            return new ChangeTrackingGradientMapAdjustment(factory.CreateGradientMapAdjustment(), changeTracker);
        }

        public IHueSaturationAdjustment CreateHueSaturationAdjustment()
        {
            return new ChangeTrackingHueSaturationAdjustment(factory.CreateHueSaturationAdjustment(), changeTracker);
        }
    }
}
