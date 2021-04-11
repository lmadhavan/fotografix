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

        public Layer CreateAdjustmentLayer()
        {
            return new Layer(new T()) { Name = name };
        }
    }
}
