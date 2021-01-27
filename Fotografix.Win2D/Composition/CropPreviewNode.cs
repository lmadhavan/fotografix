using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Fotografix.Win2D.Composition
{
    internal sealed class CropPreviewNode : IDisposable
    {
        private static readonly Color DimColor = Color.FromArgb(128, 128, 128, 128);

        private readonly CanvasGeometry dimmedArea;
        private readonly Rect cropRect;

        public CropPreviewNode(ICanvasResourceCreator resourceCreator, System.Drawing.Size imageSize, System.Drawing.Rectangle cropRectangle)
        {
            using (CanvasGeometry imageBoundsGeometry = CanvasGeometry.CreateRectangle(resourceCreator, 0, 0, imageSize.Width, imageSize.Height))
            using (CanvasGeometry cropRectangleGeometry = CanvasGeometry.CreateRectangle(resourceCreator, cropRectangle.X, cropRectangle.Y, cropRectangle.Width, cropRectangle.Height))
            {
                this.dimmedArea = imageBoundsGeometry.CombineWith(cropRectangleGeometry, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
            }

            this.cropRect = cropRectangle.ToWindowsRect();
        }

        public void Dispose()
        {
            dimmedArea.Dispose();
        }

        public void Draw(CanvasDrawingSession ds)
        {
            ds.FillGeometry(dimmedArea, DimColor);
            ds.DrawRectangle(cropRect, Colors.White);
        }
    }
}
