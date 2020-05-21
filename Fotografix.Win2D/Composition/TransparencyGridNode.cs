using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.Foundation;
using Windows.UI;

namespace Fotografix.Win2D.Composition
{
    internal sealed class TransparencyGridNode : IDisposable
    {
        private readonly CanvasCommandList gridTile;
        private readonly TileEffect tileEffect;

        public TransparencyGridNode(int gridSize)
        {
            this.gridTile = new CanvasCommandList(CanvasDevice.GetSharedDevice());
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

        public void Draw(CanvasDrawingSession ds)
        {
            ds.DrawImage(tileEffect);
        }
    }
}
