using Fotografix.Drawing;
using Fotografix.Editor.Commands;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public static class EditorProperties
    {
        private static readonly UserProperty<ICommandDispatcher> CommandDispatcherProperty = new();
        private static readonly UserProperty<Viewport> ViewportProperty = new();
        private static readonly UserProperty<IDrawable> DrawingPreviewProperty = new();
        private static readonly UserProperty<Rectangle?> CropPreviewProperty = new();

        public static string DrawingPreview => DrawingPreviewProperty.Id;
        public static string CropPreview => CropPreviewProperty.Id;

        public static Task DispatchAsync<T>(this Image image, T command)
        {
            return image.GetUserProperty(CommandDispatcherProperty).DispatchAsync(command);
        }

        public static void SetCommandDispatcher(this Image image, ICommandDispatcher commandDispatcher)
        {
            image.SetUserProperty(CommandDispatcherProperty, commandDispatcher);
        }

        public static Viewport GetViewport(this Image image)
        {
            return image.GetUserProperty(ViewportProperty);
        }

        public static void SetViewport(this Image image, Viewport viewport)
        {
            image.SetUserProperty(ViewportProperty, viewport);
        }

        public static IDrawable GetDrawingPreview(this Channel channel)
        {
            return channel.GetUserProperty(DrawingPreviewProperty);
        }

        public static void SetDrawingPreview(this Channel channel, IDrawable drawable)
        {
            channel.SetUserProperty(DrawingPreviewProperty, drawable);
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
