using Fotografix.Adjustments;
using Fotografix.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class ImageTest : ImageTestBase
    {
        private Image image;

        [TestInitialize]
        public async Task Initialize()
        {
            this.image = await LoadImageAsync("flowers.jpg");
        }

        [TestCleanup]
        public void Cleanup()
        {
            image.Dispose();
        }

        [TestMethod]
        public void ProvidesDimensions()
        {
            Assert.AreEqual(320, image.Width);
            Assert.AreEqual(480, image.Height);
        }

        [TestMethod]
        public async Task AddLayer()
        {
            Layer layer = new AdjustmentLayer(new BlackAndWhiteAdjustment());

            AssertInvalidated(() => image.Layers.Add(layer));
            await AssertImageAsync("flowers_bw.png", image);
        }

        [TestMethod]
        public async Task RemoveLayer()
        {
            Layer layer = new AdjustmentLayer(new BlackAndWhiteAdjustment());
            image.Layers.Add(layer);

            AssertInvalidated(() => image.Layers.Remove(layer));
            await AssertImageAsync("flowers.jpg", image);
        }

        [TestMethod]
        public async Task ReplaceLayer()
        {
            Layer layer1 = new AdjustmentLayer(new HueSaturationAdjustment());
            Layer layer2 = new AdjustmentLayer(new BlackAndWhiteAdjustment());

            image.Layers.Add(layer1);

            AssertInvalidated(() => image.Layers[1] = layer2);
            await AssertImageAsync("flowers_bw.png", image);
        }

        [TestMethod]
        public void InvalidateLayer()
        {
            HueSaturationAdjustment adjustment = new HueSaturationAdjustment();
            Layer layer = new AdjustmentLayer(adjustment);

            image.Layers.Add(layer);

            AssertInvalidated(() => adjustment.Hue = 1);
        }

        private void AssertInvalidated(Action action)
        {
            bool invalidated = false;
            image.Invalidated += (s, e) => invalidated = true;

            action();
            Assert.IsTrue(invalidated);
        }
    }
}
