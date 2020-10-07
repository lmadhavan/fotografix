namespace Fotografix.UI
{
    public static class BitmapLayerFactory
    {
        public static BitmapLayer CreateBitmapLayer(int id)
        {
            return new BitmapLayer(Bitmap.Empty) { Name = "Layer " + id };
        }
    }
}
