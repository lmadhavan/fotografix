﻿using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition
{
    internal sealed class DrawingPreviewNode : IComposableNode
    {
        private readonly IDrawable drawable;

        private readonly ICanvasResourceCreator resourceCreator;
        private readonly CompositeEffectNode compositeEffectNode;
        private CanvasCommandList commandList;

        internal DrawingPreviewNode(IDrawable drawable, ICanvasResourceCreator resourceCreator)
        {
            this.drawable = drawable;
            drawable.Changed += OnContentChanged;

            this.resourceCreator = resourceCreator;
            this.compositeEffectNode = new CompositeEffectNode();
            UpdateCommandList();
        }

        public void Dispose()
        {
            commandList?.Dispose();
            compositeEffectNode.Dispose();
            drawable.Changed -= OnContentChanged;
        }

        public event EventHandler Invalidated;

        public ICanvasImage Compose(ICanvasImage background)
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
            using (var dc = new Win2DDrawingContext(commandList.CreateDrawingSession()))
            {
                drawable.Draw(dc);
            }

            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}