using Fotografix.Adjustments;

namespace Fotografix
{
    public abstract class ImageElementVisitor
    {
        public virtual bool VisitEnter(Image image) => true;
        public virtual bool VisitLeave(Image image) => true;

        public virtual bool VisitEnter(AdjustmentLayer layer) => true;
        public virtual bool VisitLeave(AdjustmentLayer layer) => true;

        public virtual bool VisitEnter(BitmapLayer layer) => true;
        public virtual bool VisitLeave(BitmapLayer layer) => true;

        public virtual bool Visit(BlackAndWhiteAdjustment adjustment) => true;
        public virtual bool Visit(BrightnessContrastAdjustment adjustment) => true;
        public virtual bool Visit(GradientMapAdjustment adjustment) => true;
        public virtual bool Visit(HueSaturationAdjustment adjustment) => true;
        public virtual bool Visit(LevelsAdjustment adjustment) => true;

        public virtual bool Visit(Bitmap bitmap) => true;
    }
}
