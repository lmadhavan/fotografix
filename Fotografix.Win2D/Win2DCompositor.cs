﻿using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D
{
    public sealed class Win2DCompositor : IDisposable
    {
        private readonly ImageNode imageNode;
        private readonly TransparencyGridNode transparencyGrid;

        public Win2DCompositor(Image image)
        {
            this.imageNode = new ImageNode(image);
            this.transparencyGrid = new TransparencyGridNode(8);
        }

        public void Dispose()
        {
            transparencyGrid.Dispose();
            imageNode.Dispose();
        }

        public event EventHandler Invalidated
        {
            add => imageNode.Invalidated += value;
            remove => imageNode.Invalidated -= value;
        }

        public void Draw(CanvasDrawingSession ds)
        {
            ds.DrawImage(transparencyGrid.Output);

            ICanvasImage output = imageNode.Output;

            if (output != null)
            {
                ds.DrawImage(output);
            }
        }
    }
}