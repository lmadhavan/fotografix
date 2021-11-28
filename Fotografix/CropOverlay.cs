using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Fotografix
{
    public sealed class CropOverlay
    {
        private const float BorderThickness = 1f;
        private const float HandleThickness = 4f;
        private static readonly Color OverlayColor = Color.FromArgb(128, 0, 0, 0);

        private readonly ICanvasResourceCreator resourceCreator;
        private readonly ScalingHelper scalingHelper;

        public CropOverlay(ICanvasResourceCreator resourceCreator, ScalingHelper scalingHelper)
        {
            this.resourceCreator = resourceCreator;
            this.scalingHelper = scalingHelper;
        }

        public float HandleSize { get; set; }

        public void Draw(CanvasDrawingSession ds, CropTracker tracker)
        {
            Rect maxBounds = scalingHelper.ImageToScreen(tracker.MaxBounds);
            Rect rect = scalingHelper.ImageToScreen(tracker.Rect);

            float x = (float)rect.X;
            float y = (float)rect.Y;
            float w = (float)rect.Width;
            float h = (float)rect.Height;

            // Overlay

            using (CanvasGeometry outerGeometry = CanvasGeometry.CreateRectangle(resourceCreator, maxBounds))
            using (CanvasGeometry innerGeometry = CanvasGeometry.CreateRectangle(resourceCreator, rect))
            using (CanvasGeometry overlay = outerGeometry.CombineWith(innerGeometry, Matrix3x2.Identity, CanvasGeometryCombine.Exclude))
            {
                ds.FillGeometry(overlay, OverlayColor);
            }

            // Border

            float bt = BorderThickness;
            float hbt = BorderThickness / 2;

            ds.DrawRectangle(x - hbt, y - hbt, w + bt, h + bt, Colors.Black, hbt);
            ds.DrawRectangle(x + hbt, y + hbt, w - bt, h - bt, Colors.White, hbt);

            // Handles

            float ht = HandleThickness;
            float hs = HandleSize;

            if (w < 2 * hs || h < 2 * hs)
            {
                return;
            }

            void DrawHandle(float x0, float y0, float x1, float y1)
            {
                ds.DrawLine(x0, y0, x1, y1, Colors.White, ht);
            }

            DrawHandle(x, y, x + hs, y);                    // top-left
            DrawHandle(x, y, x, y + hs);                    // top-left
            DrawHandle(x + w - hs, y, x + w, y);            // top-right
            DrawHandle(x + w, y, x + w, y + hs);            // top-right
            DrawHandle(x, y + h, x + hs, y + h);            // bottom-left
            DrawHandle(x, y + h, x, y + h - hs);            // bottom-left
            DrawHandle(x + w - hs, y + h, x + w, y + h);    // bottom-right
            DrawHandle(x + w, y + h, x + w, y + h - hs);    // bottom-right

            // Guides

            void DrawHGuide(float pos)
            {
                float gy = y + pos * h;
                ds.DrawLine(x, gy - 0.25f, x + w, gy - 0.25f, Colors.Black, 0.5f);
                ds.DrawLine(x, gy + 0.25f, x + w, gy + 0.25f, Colors.White, 0.5f);
            }

            void DrawVGuide(float pos)
            {
                float gx = x + pos * w;
                ds.DrawLine(gx - 0.25f, y, gx - 0.25f, y + h, Colors.Black, 0.5f);
                ds.DrawLine(gx + 0.25f, y, gx + 0.25f, y + h, Colors.White, 0.5f);
            }

            DrawHGuide(0.33f);
            DrawHGuide(0.67f);
            DrawVGuide(0.33f);
            DrawVGuide(0.67f);
        }
    }
}
