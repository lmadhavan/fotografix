using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor
{
    [TestFixture]
    public class ResizeImageParametersTest
    {
        [Test]
        public void AspectRatioLocked_ChangingWidthShouldChangeHeight()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = true;
            parameters.Width = 10;

            Assert.AreEqual(5, parameters.Height);
        }

        [Test]
        public void AspectRatioLocked_ChangingHeightShouldChangeWidth()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = true;
            parameters.Height = 5;

            Assert.AreEqual(10, parameters.Width);
        }

        [Test]
        public void AspectRatioUnlocked_ChangingWidthShouldNotChangeHeight()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = false;
            parameters.Width = 10;

            Assert.AreEqual(10, parameters.Height);
        }

        [Test]
        public void AspectRatioUnlocked_ChangingHeightShouldNotChangeWidth()
        {
            ResizeImageParameters parameters = new ResizeImageParameters(new Size(20, 10));
            parameters.LockAspectRatio = false;
            parameters.Height = 5;

            Assert.AreEqual(20, parameters.Width);
        }

        [Test]
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
