using Fotografix.Adjustments;
using System;

namespace Fotografix.UI.Adjustments
{
    public sealed class AdjustmentLayerFactory
    {
        private readonly string name;
        private readonly Func<IAdjustmentFactory, IAdjustment> createFunc;

        public AdjustmentLayerFactory(string name, Func<IAdjustmentFactory, IAdjustment> createFunc)
        {
            this.name = name;
            this.createFunc = createFunc;
        }

        public AdjustmentLayer CreateAdjustmentLayer(IAdjustmentFactory adjustmentFactory)
        {
            IAdjustment adjustment = createFunc(adjustmentFactory);
            return new AdjustmentLayer(adjustment) { Name = name };
        }
    }
}
