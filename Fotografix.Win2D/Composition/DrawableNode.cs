﻿using Fotografix.Drawing;
using Fotografix.Win2D.Drawing;
using Microsoft.Graphics.Canvas;
using System;
using System.Drawing;

namespace Fotografix.Win2D.Composition
{
    internal sealed class DrawableNode : IDisposable
    {
        private readonly IWin2DDrawable drawable;
        private readonly Rectangle bounds;

        private readonly ICanvasResourceCreator resourceCreator;
        private readonly CompositeEffectNode compositeEffectNode;
        private CanvasCommandList commandList;

        internal DrawableNode(IDrawable drawable, Rectangle bounds)
        {
            this.drawable = (IWin2DDrawable)drawable;
            drawable.ContentChanged += OnContentChanged;

            this.bounds = bounds;

            this.resourceCreator = CanvasDevice.GetSharedDevice();
            this.compositeEffectNode = new CompositeEffectNode();
            UpdateCommandList();
        }

        public void Dispose()
        {
            commandList?.Dispose();
            compositeEffectNode.Dispose();
            drawable.ContentChanged -= OnContentChanged;
        }

        public event EventHandler OutputChanged;

        public ICanvasImage ResolveOutput(ICanvasImage background)
        {
            return compositeEffectNode.ResolveOutput(commandList, background);
        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            UpdateCommandList();
        }

        private void UpdateCommandList()
        {
            commandList?.Dispose();

            this.commandList = new CanvasCommandList(resourceCreator);
            using (CanvasDrawingSession ds = commandList.CreateDrawingSession())
            {
                drawable.Draw(ds, bounds);
            }

            OutputChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}