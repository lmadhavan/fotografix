﻿using Fotografix.Drawing;
using Fotografix.Editor.Commands;
using Moq;
using NUnit.Framework;
using System;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingToolTest : ChannelToolTest
    {
        private static readonly PointerState Start = new(10, 10);
        private static readonly PointerState End = new(20, 20);
        private static readonly Rectangle Selection = new(5, 5, 10, 10);

        private Mock<ICommandDispatcher> commandDispatcher;

        protected abstract void AssertDrawable(IDrawable drawable, PointerState start, PointerState end);

        [SetUp]
        public void SetUp_DrawingToolTest()
        {
            this.commandDispatcher = new Mock<ICommandDispatcher>();
            Image.SetCommandDispatcher(commandDispatcher.Object);
            Image.Selection = Selection;
        }

        [Test]
        public void BeginsDrawingPreviewWhenPointerPressed()
        {
            Activate(BitmapLayer);

            Tool.PointerPressed(Start);
            
            Assert.That(ActiveChannel.GetDrawingPreview(), Is.Not.Null);
        }

        [Test]
        public void UpdatesDrawingWhenPointerMoved()
        {
            Activate(BitmapLayer);

            Tool.PointerPressed(Start);
            Tool.PointerMoved(End);

            AssertClippedDrawable(ActiveChannel.GetDrawingPreview(), Start, End);
        }

        [Test]
        public void CommitsDrawingWhenPointerReleased()
        {
            Activate(BitmapLayer);

            Tool.PointerPressed(Start);
            Tool.PointerMoved(End);

            IDrawable drawable = ActiveChannel.GetDrawingPreview();

            Tool.PointerReleased(End);

            commandDispatcher.Verify(d => d.DispatchAsync(new DrawCommand(BitmapLayer.ContentChannel, drawable)));
            Assert.That(ActiveChannel.GetDrawingPreview(), Is.Null);
        }

        [Test]
        public void DoesNotUpdateDrawingAfterPointerReleased()
        {
            Activate(BitmapLayer);

            Tool.PointerPressed(Start);
            Tool.PointerMoved(End);

            IDrawable drawable = ActiveChannel.GetDrawingPreview();

            Tool.PointerReleased(End);
            Tool.PointerMoved(new(30, 30));

            AssertClippedDrawable(drawable, Start, End);
        }

        private void AssertClippedDrawable(IDrawable drawable, PointerState start, PointerState end)
        {
            Assert.That(drawable, Is.InstanceOf<ClippedDrawable>());

            ClippedDrawable clippedDrawable = (ClippedDrawable)drawable;
            Assert.That(clippedDrawable.ClipRectangle, Is.EqualTo(Selection));
            AssertDrawable(clippedDrawable.WrappedDrawable, start, end);
        }
    }
}
