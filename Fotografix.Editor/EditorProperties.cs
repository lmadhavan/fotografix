﻿using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Editor
{
    public static class EditorProperties
    {
        private static readonly UserProperty<IDrawable> DrawingPreviewProperty = new();
        private static readonly UserProperty<Rectangle?> CropPreviewProperty = new();

        public static string DrawingPreview => DrawingPreviewProperty.Id;
        public static string CropPreview => CropPreviewProperty.Id;

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
