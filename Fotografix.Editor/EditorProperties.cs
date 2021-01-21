using Fotografix.Drawing;

namespace Fotografix.Editor
{
    public static class EditorProperties
    {
        private static readonly UserPropertyKey<Layer> ActiveLayer = new UserPropertyKey<Layer>();
        private static readonly UserPropertyKey<ICommandDispatcher> CommandDispatcher = new UserPropertyKey<ICommandDispatcher>();
        private static readonly UserPropertyKey<IDrawable> DrawingPreview = new UserPropertyKey<IDrawable>();

        public static IUserPropertyKey ActiveLayerProperty => ActiveLayer;
        public static IUserPropertyKey DrawingPreviewProperty => DrawingPreview;

        public static Layer GetActiveLayer(this Image image)
        {
            return image.GetUserProperty(ActiveLayer);
        }

        public static void SetActiveLayer(this Image image, Layer layer)
        {
            image.SetUserProperty(ActiveLayer, layer);
        }

        public static void Dispatch<T>(this Image image, T command) where T : ICommand
        {
            image.GetUserProperty(CommandDispatcher).Dispatch(command);
        }

        public static void SetCommandDispatcher(this Image image, ICommandDispatcher commandDispatcher)
        {
            image.SetUserProperty(CommandDispatcher, commandDispatcher);
        }

        public static IDrawable GetDrawingPreview(this Layer layer)
        {
            return layer.GetUserProperty(DrawingPreview);
        }

        public static void SetDrawingPreview(this Layer layer, IDrawable drawable)
        {
            layer.SetUserProperty(DrawingPreview, drawable);
        }
    }
}
