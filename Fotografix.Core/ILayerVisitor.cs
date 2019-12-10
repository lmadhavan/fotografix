namespace Fotografix
{
    public interface ILayerVisitor
    {
        void Visit(AdjustmentLayer layer);
        void Visit(BitmapLayer layer);
    }
}