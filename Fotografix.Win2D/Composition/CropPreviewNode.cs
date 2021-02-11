using Fotografix.Editor;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Fotografix.Win2D.Composition
{
    internal sealed class CropPreviewNode : IDisposable
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

        private readonly Viewport viewport;
        private readonly Rectangle viewportCropBounds;
        private readonly CanvasGeometry dimmedArea;

        public CropPreviewNode(ICompositionRoot root, Rectangle cropRectangle)
        {
            this.viewport = root.Viewport;
            this.viewportCropBounds = viewport.TransformImageToViewport(cropRectangle);

            using (CanvasGeometry imageBoundsGeometry = CanvasGeometry.CreateRectangle(root.ResourceCreator, viewport.ImageBounds.ToWindowsRect()))
            using (CanvasGeometry cropRectangleGeometry = CanvasGeometry.CreateRectangle(root.ResourceCreator, viewportCropBounds.ToWindowsRect()))
            {
                this.dimmedArea = imageBoundsGeometry.CombineWith(cropRectangleGeometry, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
            }
        }

        public void Dispose()
        {
            dimmedArea.Dispose();
        }

        public void Draw(CanvasDrawingSession ds)
        {
            void OuterStroke(RectangleF r)
            {
                ds.DrawRectangle(r.X - 0.5f, r.Y - 0.5f, r.Width + 1f, r.Height + 1f, OuterStrokeColor);
            }

            void InnerStroke(RectangleF r)
            {
                ds.DrawRectangle(r.X + 0.5f, r.Y + 0.5f, r.Width - 1f, r.Height - 1f, InnerStrokeColor);
            }

            void Fill(RectangleF r)
            {
                ds.FillRectangle(r.X, r.Y, r.Width, r.Height, FillColor);
            }

            ds.FillGeometry(dimmedArea, DimColor);

            OuterStroke(viewportCropBounds);
            InnerStroke(viewportCropBounds);

            foreach (PointF hl in HandleLocations)
            {
                RectangleF handleRect = TransformHandleLocationToViewportRect(hl);
                OuterStroke(handleRect);
                Fill(handleRect);
            }
        }

        private RectangleF TransformHandleLocationToViewportRect(PointF hl)
        {
            PointF center = new PointF(
                viewportCropBounds.X + hl.X * viewportCropBounds.Width,
                viewportCropBounds.Y + hl.Y * viewportCropBounds.Height
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
