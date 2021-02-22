using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Foundation;
using Windows.UI;

namespace Fotografix.Win2D.Composition
{
    internal sealed class TransparencyGridNode : IDrawableNode
    {
        private readonly CanvasCommandList gridTile;
        private readonly TileEffect tileEffect;

        public TransparencyGridNode(int gridSize, ICanvasResourceCreator resourceCreator)
        {
            this.gridTile = new CanvasCommandList(resourceCreator);
            using (CanvasDrawingSession ds = gridTile.CreateDrawingSession())
            {
                ds.Clear(Colors.White);
                ds.FillRectangle(new Rect(gridSize, 0, gridSize, gridSize), Colors.LightGray);
                ds.FillRectangle(new Rect(0, gridSize, gridSize, gridSize), Colors.LightGray);
            }

            this.tileEffect = new TileEffect()
            {
                Source = gridTile,
                SourceRectangle = new Rect(0, 0, gridSize * 2, gridSize * 2)
            };
        }

        public void Dispose()
        {
            tileEffect.Dispose();
            gridTile.Dispose();
        }

        public void Draw(CanvasDrawingSession ds, Rect imageBounds)
        {
            ds.DrawImage(tileEffect, imageBounds, new Rect(0, 0, imageBounds.Width, imageBounds.Height));
        }
    }
}
