﻿using Fotografix.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class ResizeImageAcceptanceTest : AcceptanceTestBase
    {
        [TestMethod]
        public async Task ResizingImageScalesBitmapLayers()
        {
            await OpenImageAsync("flowers.jpg");

            var parameters = BeginResizeImage();
            parameters.LockAspectRatio = true;
            parameters.Width /= 2;
            await ResizeImageAsync(parameters);

            await AssertImageAsync("flowers_scale50.png");
        }

        private ResizeImageParameters BeginResizeImage()
        {
            return Editor.CreateResizeImageParameters();
        }

        private Task ResizeImageAsync(ResizeImageParameters parameters)
        {
            return Editor.ResizeImageAsync(parameters);
        }
    }
}
