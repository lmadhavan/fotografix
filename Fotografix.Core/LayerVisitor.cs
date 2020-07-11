namespace Fotografix
{
    public abstract class LayerVisitor
    {
        public virtual void Visit(AdjustmentLayer layer) { }
        public virtual void Visit(BitmapLayer layer) { }
    }
}