using System;
using System.Drawing;

namespace Fotografix.Drawing
{
    public sealed class LinearGradient : IDrawable
    {
        private Point startPoint;
        private Point endPoint;

        public LinearGradient(Rectangle bounds, Color startColor, Color endColor)
        {
            this.Bounds = bounds;
            this.StartColor = startColor;
            this.EndColor = endColor;
        }

        public Rectangle Bounds { get; }
        public Color StartColor { get; }
        public Color EndColor { get; }

        public Point StartPoint
        {
            get => startPoint;

            set
            {
                this.startPoint = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public Point EndPoint
        {
            get => endPoint;

            set
            {
                this.endPoint = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Changed;

        public void Draw(IDrawingContext drawingContext)
        {
            drawingContext.Draw(this);
        }
    }
}
