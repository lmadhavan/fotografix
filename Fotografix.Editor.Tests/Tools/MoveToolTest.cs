﻿using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class MoveToolTest : ChannelToolTest
    {
        private MoveTool tool;

        protected override ITool Tool => tool;

        [SetUp]
        public void SetUp()
        {
            this.tool = new MoveTool();
        }

        [Test]
        public void MovesActiveBitmapWhenPointerIsDragged()
        {
            Activate(BitmapLayer);
            ActiveChannel.Position = new(50, 50);

            tool.PointerPressed(new(10, 10));
            tool.PointerMoved(new(15, 15));

            Assert.That(ActiveChannel.Position, Is.EqualTo(new Point(55, 55)));
        }

        [Test]
        public void DoesNotMoveBitmapAfterPointerIsReleased()
        {
            Activate(BitmapLayer);

            tool.PointerPressed(new(10, 10));
            tool.PointerReleased(new(10, 10));
            tool.PointerMoved(new(15, 15));

            Assert.That(ActiveChannel.Position, Is.EqualTo(Point.Empty));
        }
    }
}
