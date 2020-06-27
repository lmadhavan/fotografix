using System.Drawing;

namespace Fotografix.Editor.Testing
{
    public class FakeViewport : Viewport
    {
        public FakeViewport() : this(0, 0)
        {
        }

        public FakeViewport(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.ZoomFactor = 1;
        }

        public override int Width { get; }
        public override int Height { get; }

        public override float ZoomFactor { get; set; }
        public override PointF ScrollOffset { get; set; }
    }
}
