using Fotografix.Editor;
using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;

namespace Fotografix.Win2D
{
    public sealed class Win2DCompositor : IDisposable
    {
        private readonly Viewport viewport;
        private readonly NodeFactory nodeFactory;
        private readonly List<IDrawableNode> nodes;

        public Win2DCompositor(Image image, Viewport viewport, Win2DCompositorSettings settings)
        {
            this.viewport = viewport;
            viewport.PropertyChanged += Viewport_PropertyChanged;

            this.nodeFactory = settings.CreateNodeFactory();

            this.nodes = new List<IDrawableNode>();
            nodes.Add(nodeFactory.CreateTransparencyGridNode());
            nodes.Add(nodeFactory.CreateImageNode(image));
            nodes.Add(nodeFactory.CreateCropPreviewNode(image, viewport));
        }

        public void Dispose()
        {
            foreach (var node in nodes)
            {
                node.Dispose();
            }

            viewport.PropertyChanged -= Viewport_PropertyChanged;
        }

        public event EventHandler Invalidated
        {
            add => nodeFactory.Invalidated += value;
            remove => nodeFactory.Invalidated -= value;
        }

        public void Draw(CanvasDrawingSession ds)
        {
            Rect imageBounds = viewport.ImageBounds.ToWindowsRect();

            foreach (var node in nodes)
            {
                node.Draw(ds, imageBounds);
            }
        }

        public void Draw(Bitmap bitmap)
        {
            using (Win2DBitmap win2DBitmap = nodeFactory.WrapBitmap(bitmap))
            {
                win2DBitmap.Draw(this);
            }
        }

        public Bitmap ToBitmap()
        {
            Win2DBitmap bitmap = nodeFactory.CreateBitmap(viewport.Size);
            bitmap.Draw(this);
            return bitmap.Source;
        }

        private void Viewport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            nodeFactory.Invalidate();
        }
    }
}
