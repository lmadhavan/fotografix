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

        private readonly CanvasGeometry dimmedArea;
        private readonly Rectangle cropRect;

        public CropPreviewNode(ICanvasResourceCreator resourceCreator, Size imageSize, Rectangle cropRectangle)
        {
            using (CanvasGeometry imageBoundsGeometry = CanvasGeometry.CreateRectangle(resourceCreator, 0, 0, imageSize.Width, imageSize.Height))
            using (CanvasGeometry cropRectangleGeometry = CanvasGeometry.CreateRectangle(resourceCreator, cropRectangle.X, cropRectangle.Y, cropRectangle.Width, cropRectangle.Height))
            {
                this.dimmedArea = imageBoundsGeometry.CombineWith(cropRectangleGeometry, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
            }

            this.cropRect = cropRectangle;
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

            OuterStroke(cropRect);
            InnerStroke(cropRect);

            foreach (PointF hl in HandleLocations)
            {
                RectangleF r = GetHandleRectangle(hl);
                OuterStroke(r);
                Fill(r);
            }
        }

        private RectangleF GetHandleRectangle(PointF hl)
        {
            PointF pt = new PointF(
                cropRect.X + hl.X * cropRect.Width,
                cropRect.Y + hl.Y * cropRect.Height
            );

            return RectangleF.FromLTRB(
                pt.X - HandleSize / 2,
                pt.Y - HandleSize / 2,
                pt.X + HandleSize / 2,
                pt.Y + HandleSize / 2
            );
        }
    }
}
