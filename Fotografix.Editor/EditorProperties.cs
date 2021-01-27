using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Editor
{
    public static class EditorProperties
    {
        private static readonly UserProperty<Layer> ActiveLayerProperty = new UserProperty<Layer>();
        private static readonly UserProperty<ICommandDispatcher> CommandDispatcherProperty = new UserProperty<ICommandDispatcher>();
        private static readonly UserProperty<IDrawable> DrawingPreviewProperty = new UserProperty<IDrawable>();
        private static readonly UserProperty<Rectangle?> CropPreviewProperty = new UserProperty<Rectangle?>();

        public static string ActiveLayer => ActiveLayerProperty.Id;
        public static string DrawingPreview => DrawingPreviewProperty.Id;
        public static string CropPreview => CropPreviewProperty.Id;

        public static Layer GetActiveLayer(this Image image)
        {
            return image.GetUserProperty(ActiveLayerProperty);
        }

        public static void SetActiveLayer(this Image image, Layer layer)
        {
            image.SetUserProperty(ActiveLayerProperty, layer);
        }

        public static void Dispatch<T>(this Image image, T command) where T : ICommand
        {
            image.GetUserProperty(CommandDispatcherProperty).Dispatch(command);
        }

        public static void SetCommandDispatcher(this Image image, ICommandDispatcher commandDispatcher)
        {
            image.SetUserProperty(CommandDispatcherProperty, commandDispatcher);
        }

        public static IDrawable GetDrawingPreview(this Layer layer)
        {
            return layer.GetUserProperty(DrawingPreviewProperty);
        }

        public static void SetDrawingPreview(this Layer layer, IDrawable drawable)
        {
            layer.SetUserProperty(DrawingPreviewProperty, drawable);
        }

        public static Rectangle? GetCropPreview(this Image image)
        {
            return image.GetUserProperty(CropPreviewProperty);
        }

        public static void SetCropPreview(this Image image, Rectangle? rectangle)
        {
            image.SetUserProperty(CropPreviewProperty, rectangle);
        }
    }
}
