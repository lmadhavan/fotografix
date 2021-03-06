﻿using Fotografix.Adjustments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class AdjustmentBlendingTest : CompositionTestBase
    {
        private Layer background;
        private Layer foreground;

        [TestInitialize]
        public void Initialize()
        {
            this.background = Image.Layers[0];
            this.foreground = new Layer(new BlackAndWhiteAdjustment());
            Image.Layers.Add(foreground);
        }

        [TestMethod]
        public async Task DefaultProperties()
        {
            await AssertImageAsync("flowers_bw.png");
        }

        [TestMethod]
        public async Task Foreground_Visible_False()
        {
            foreground.Visible = false;

            await AssertImageAsync("flowers.jpg");
        }

        [TestMethod]
        public async Task Foreground_Opacity_0()
        {
            foreground.Opacity = 0;

            await AssertImageAsync("flowers.jpg");
        }

        [TestMethod]
        public async Task Foreground_Opacity_50()
        {
            foreground.Opacity = 0.5f;

            await AssertImageAsync("flowers_bw_opacity50.png");
        }

        [TestMethod]
        public async Task Foreground_BlendMode_Multiply()
        {
            foreground.BlendMode = BlendMode.Multiply;

            await AssertImageAsync("flowers_bw_multiply.png");
        }

        [TestMethod]
        public async Task Foreground_BlendMode_Multiply_Opacity_50()
        {
            foreground.BlendMode = BlendMode.Multiply;
            foreground.Opacity = 0.5f;

            await AssertImageAsync("flowers_bw_multiply_opacity50.png");
        }

        [TestMethod]
        public async Task Background_Visible_False()
        {
            background.Visible = false;

            await AssertImageAsync("empty.png");
        }

        [TestMethod]
        public async Task Background_Opacity_0()
        {
            background.Opacity = 0;

            await AssertImageAsync("empty.png");
        }

        [TestMethod]
        public async Task Background_Opacity_50()
        {
            background.Opacity = 0.5f;

            await AssertImageAsync("flowers_opacity50_bw.png");
        }

        [TestMethod]
        public async Task Both_Visible_False()
        {
            foreground.Visible = false;
            background.Visible = false;

            await AssertImageAsync("empty.png");
        }
    }
}
