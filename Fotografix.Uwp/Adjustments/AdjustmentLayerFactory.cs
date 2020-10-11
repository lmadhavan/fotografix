using Fotografix.Adjustments;

namespace Fotografix.Uwp.Adjustments
{
    public sealed class AdjustmentLayerFactory<T> : IAdjustmentLayerFactory where T : Adjustment, new()
    {
        private readonly string name;

        public AdjustmentLayerFactory(string name)
        {
            this.name = name;
        }

        public AdjustmentLayer CreateAdjustmentLayer()
        {
            return new AdjustmentLayer(new T()) { Name = name };
        }
    }
}
