using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Editor.Colors
{
    public class ColorControlsTest
    {
        private ColorControls colors;
        private List<string> updatedProperties;

        [SetUp]
        public void SetUp()
        {
            this.colors = new();
            this.updatedProperties = new();
            colors.PropertyChanged += (s, e) => updatedProperties.Add(e.PropertyName);
        }

        [Test]
        public void UpdatesForegroundColor()
        {
            colors.IsForegroundColorActive = true;
            Assert.IsFalse(colors.IsBackgroundColorActive);

            updatedProperties.Clear();

            colors.ActiveColor = Color.Red;
            Assert.That(colors.ForegroundColor, Is.EqualTo(Color.Red));
            Assert.That(colors.BackgroundColor, Is.EqualTo(ColorControls.DefaultBackgroundColor));

            AssertUpdatedProperties(
                nameof(colors.ActiveColor),
                nameof(colors.ForegroundColor)
            );
        }

        [Test]
        public void UpdatesBackgroundColor()
        {
            colors.IsBackgroundColorActive = true;
            Assert.IsFalse(colors.IsForegroundColorActive);

            updatedProperties.Clear();

            colors.ActiveColor = Color.Red;
            Assert.That(colors.BackgroundColor, Is.EqualTo(Color.Red));
            Assert.That(colors.ForegroundColor, Is.EqualTo(ColorControls.DefaultForegroundColor));

            AssertUpdatedProperties(
                nameof(colors.ActiveColor),
                nameof(colors.BackgroundColor)
            );
        }

        private void AssertUpdatedProperties(params string[] properties)
        {
            foreach (string prop in properties)
            {
                Assert.That(updatedProperties, Contains.Item(prop));
            }
        }
    }
}
