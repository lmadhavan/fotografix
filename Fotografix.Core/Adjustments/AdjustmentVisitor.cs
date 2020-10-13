namespace Fotografix.Adjustments
{
    public abstract class AdjustmentVisitor
    {
        public virtual void Visit(BlackAndWhiteAdjustment adjustment) { }
        public virtual void Visit(BrightnessContrastAdjustment adjustment) { }
        public virtual void Visit(GradientMapAdjustment adjustment) { }
        public virtual void Visit(HueSaturationAdjustment adjustment) { }
        public virtual void Visit(LevelsAdjustment adjustment) { }
    }
}
