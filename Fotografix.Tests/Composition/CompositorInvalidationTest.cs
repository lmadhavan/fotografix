using Fotografix.Adjustments;
using Fotografix.Win2D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class CompositorInvalidationTest : CompositionTestBase
    {
        private HueSaturationAdjustment adjustment;
        private Layer layer;
        private Image image;
        private Win2DCompositor compositor;

        [TestInitialize]
        public void Initialize()
        {
            this.adjustment = new HueSaturationAdjustment();
            this.layer = new AdjustmentLayer(adjustment);
            
            this.image = new Image(new Size(10, 10));
            image.Layers.Add(layer);

            this.compositor = new Win2DCompositor(image);
        }

        [TestCleanup]
        public void Cleanup()
        {
            compositor.Dispose();
        }

        [TestMethod]
        public void InvalidatesWhenLayerAdded()
        {
            AssertInvalidated(() => image.Layers.Add(layer));
        }

        [TestMethod]
        public void InvalidatesWhenLayerRemoved()
        {
            AssertInvalidated(() => image.Layers.Remove(layer));
        }

        [TestMethod]
        public void InvalidatesWhenLayerPropertyChanged()
        {
            AssertInvalidated(() => layer.Opacity = 0.5f);
        }

        [TestMethod]
        public void InvalidatesWhenAdjustmentPropertyChanged()
        {
            AssertInvalidated(() => adjustment.Hue = 0.5f);
        }

        private void AssertInvalidated(Action action)
        {
            bool invalidated = false;
            compositor.Invalidated += (s, e) => invalidated = true;

            action();
            Assert.IsTrue(invalidated);
        }
    }
}
