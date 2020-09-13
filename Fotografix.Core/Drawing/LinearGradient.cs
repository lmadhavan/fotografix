using System.Drawing;

namespace Fotografix.Drawing
{
    public sealed class LinearGradient : NotifyContentChangedBase, IDrawable
    {
        public LinearGradient(Color startColor, Color endColor, Point startPoint)
        {
            this.StartColor = startColor;
            this.EndColor = endColor;
            this.StartPoint = startPoint;
        }

        public Color StartColor { get; }
        public Color EndColor { get; }
        public Point StartPoint { get; }
        public Point EndPoint { get; private set; }

        public void SetEndPoint(Point pt)
        {
            this.EndPoint = pt;
            RaiseContentChanged();
        }

        public void Draw(IDrawingContext drawingContext)
        {
            drawingContext.Draw(this);
        }
    }
}
