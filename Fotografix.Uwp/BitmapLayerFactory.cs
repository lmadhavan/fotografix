namespace Fotografix.Uwp
{
    public static class BitmapLayerFactory
    {
        public static BitmapLayer CreateBitmapLayer(int id)
        {
            return new BitmapLayer { Name = "Layer " + id };
        }
    }
}
