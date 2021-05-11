using Fotografix.Drawing;
using Fotografix.Editor;
using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition
{
    internal sealed class DrawingPreviewNode : IComposableNode
    {
        private readonly Channel channel;
        private readonly ICanvasResourceCreator resourceCreator;
        private readonly CompositeEffectNode compositeEffectNode;

        private IDrawable drawable;
        private CanvasCommandList commandList;

        internal DrawingPreviewNode(Channel channel, ICanvasResourceCreator resourceCreator)
        {
            this.channel = channel;
            this.resourceCreator = resourceCreator;
            this.compositeEffectNode = new CompositeEffectNode();

            UpdateDrawable();
            channel.UserPropertyChanged += Channel_UserPropertyChanged;
        }

        public void Dispose()
        {
            channel.UserPropertyChanged -= Channel_UserPropertyChanged;

            if (drawable != null)
            {
                drawable.Changed -= Drawable_Changed;
            }

            commandList?.Dispose();
            compositeEffectNode.Dispose();
        }

        public event EventHandler Invalidated;

        public ICanvasImage Compose(ICanvasImage background)
        {
            if (commandList != null)
            {
                return compositeEffectNode.ResolveOutput(commandList, background);
            }

            return background;
        }

        private void UpdateDrawable()
        {
            if (drawable != null)
            {
                drawable.Changed -= Drawable_Changed;
            }

            this.drawable = channel.GetDrawingPreview();

            if (drawable != null)
            {
                drawable.Changed += Drawable_Changed;
            }

            UpdateCommandList();
        }

        private void UpdateCommandList()
        {
            commandList?.Dispose();
            this.commandList = null;

            if (drawable != null)
            {
                this.commandList = new CanvasCommandList(resourceCreator);
                using (var dc = new Win2DDrawingContext(commandList.CreateDrawingSession()))
                {
                    drawable.Draw(dc);
                }
            }

            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        private void Channel_UserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == EditorProperties.DrawingPreview)
            {
                UpdateDrawable();
            }
        }

        private void Drawable_Changed(object sender, EventArgs e)
        {
            UpdateCommandList();
        }
    }
}