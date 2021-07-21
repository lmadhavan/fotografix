using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Drawing
{
    public sealed class TestGraphicsDevice : IDrawingContext, IGraphicsDevice
    {
        private readonly List<object> calls = new();

        public IReadOnlyList<object> Calls => calls;

        public void VerifySequence(params object[] expected)
        {
            Assert.That(calls, Is.EqualTo(expected).AsCollection);
        }

        public IDrawingContext CreateDrawingContext(Bitmap bitmap)
        {
            calls.Add(("CreateDrawingContext", bitmap));
            return this;
        }

        public void Dispose()
        {
            calls.Add("DisposeDrawingContext");
        }

        public IDisposable BeginClip(Rectangle rect)
        {
            calls.Add(("BeginClip", rect));
            return new DisposableAction(EndClip);
        }

        private void EndClip()
        {
            calls.Add("EndClip");
        }

        public void Draw(TestDrawable drawable)
        {
            calls.Add(("Draw", drawable));
        }

        public void Draw(Bitmap bitmap, Rectangle destRect, Rectangle srcRect)
        {
            calls.Add(("Draw", bitmap, destRect, srcRect));
        }

        public void Draw(BrushStroke brushStroke)
        {
            calls.Add(("Draw", brushStroke));
        }

        public void Draw(LinearGradient gradient)
        {
            calls.Add(("Draw", gradient));
        }
    }
}
