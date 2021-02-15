using Fotografix.Editor;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Windows.Foundation;

namespace Fotografix.Win2D.Composition
{
    internal sealed class CropPreviewNode
    {
        private const int HandleSize = 8;

        private static readonly Windows.UI.Color DimColor = Windows.UI.Color.FromArgb(128, 128, 128, 128);
        private static readonly Windows.UI.Color OuterStrokeColor = Windows.UI.Colors.Black;
        private static readonly Windows.UI.Color InnerStrokeColor = Windows.UI.Colors.White;
        private static readonly Windows.UI.Color FillColor = Windows.UI.Colors.White;

        private static readonly List<PointF> HandleLocations = new List<PointF>
        {
            new PointF(0.0f, 0.0f),
            new PointF(0.5f, 0.0f),
            new PointF(1.0f, 0.0f),
            new PointF(0.0f, 0.5f),
            new PointF(1.0f, 0.5f),
            new PointF(0.0f, 1.0f),
            new PointF(0.5f, 1.0f),
            new PointF(1.0f, 1.0f)
        };

        private readonly Image image;
        private readonly ICanvasResourceCreator resourceCreator;
        private readonly Viewport viewport;

        public CropPreviewNode(Image image, ICanvasResourceCreator resourceCreator, Viewport viewport)
        {
            this.image = image;
            this.resourceCreator = resourceCreator;
            this.viewport = viewport;
        }

        public void Draw(CanvasDrawingSession ds, Rect imageBounds)
        {
            var cropPreview = image.GetCropPreview();

            if (cropPreview != null)
            {
                Rectangle cropBounds = viewport.TransformImageToViewport(cropPreview.Value);
                Draw(ds, imageBounds, cropBounds);
            }
        }

        private void Draw(CanvasDrawingSession ds, Rect imageBounds, Rectangle cropBounds)
        {
            void OuterStroke(RectangleF r) =>ds.DrawRectangle(r.X - 0.5f, r.Y - 0.5f, r.Width + 1f, r.Height + 1f, OuterStrokeColor);
            void InnerStroke(RectangleF r) => ds.DrawRectangle(r.X + 0.5f, r.Y + 0.5f, r.Width - 1f, r.Height - 1f, InnerStrokeColor);
            void Fill(RectangleF r) => ds.FillRectangle(r.X, r.Y, r.Width, r.Height, FillColor);

            HighlightCropRectangle(ds, imageBounds, cropBounds.ToWindowsRect());
            OuterStroke(cropBounds);
            InnerStroke(cropBounds);

            foreach (PointF hl in HandleLocations)
            {
                RectangleF handleRect = HandleLocationToRectangle(cropBounds, hl);
                OuterStroke(handleRect);
                Fill(handleRect);
            }
        }

        private void HighlightCropRectangle(CanvasDrawingSession ds, Rect imageBounds, Rect cropBounds)
        {
            using (CanvasGeometry imageBoundsGeometry = CanvasGeometry.CreateRectangle(resourceCreator, imageBounds))
            using (CanvasGeometry cropRectangleGeometry = CanvasGeometry.CreateRectangle(resourceCreator, cropBounds))
            using (CanvasGeometry dimmedArea = imageBoundsGeometry.CombineWith(cropRectangleGeometry, Matrix3x2.Identity, CanvasGeometryCombine.Exclude))
            {
                ds.FillGeometry(dimmedArea, DimColor);
            }
        }

        private RectangleF HandleLocationToRectangle(RectangleF bounds, PointF hl)
        {
            PointF center = new PointF(
                bounds.X + hl.X * bounds.Width,
                bounds.Y + hl.Y * bounds.Height
            );

            return RectangleF.FromLTRB(
                center.X - HandleSize / 2,
                center.Y - HandleSize / 2,
                center.X + HandleSize / 2,
                center.Y + HandleSize / 2
            );
        }
    }
}
