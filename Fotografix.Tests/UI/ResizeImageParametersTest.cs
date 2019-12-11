using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class ResizeImageParametersTest
    {
        [TestMethod]
        public void AspectRatioLocked_ChangingWidthShouldChangeHeight()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = true;
            parameters.Width = 10;

            Assert.AreEqual(5, parameters.Height);
        }

        [TestMethod]
        public void AspectRatioLocked_ChangingHeightShouldChangeWidth()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = true;
            parameters.Height = 5;

            Assert.AreEqual(10, parameters.Width);
        }

        [TestMethod]
        public void AspectRatioUnlocked_ChangingWidthShouldNotChangeHeight()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = false;
            parameters.Width = 10;

            Assert.AreEqual(10, parameters.Height);
        }

        [TestMethod]
        public void AspectRatioUnlocked_ChangingHeightShouldNotChangeWidth()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = false;
            parameters.Height = 5;

            Assert.AreEqual(20, parameters.Width);
        }

        [TestMethod]
        public void LockingAspectRatioUpdatesExistingDimensions()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = false;
            parameters.Width = 10;
            parameters.LockAspectRatio = true;

            Assert.AreEqual(5, parameters.Height);
        }
    }
}
