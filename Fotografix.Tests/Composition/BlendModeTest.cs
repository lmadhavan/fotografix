using Fotografix.Composition;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class BlendModeTest
    {
        [TestMethod]
        public void MapsToWin2DBlendModes()
        {
            foreach (BlendMode mode in Enum.GetValues(typeof(BlendMode)))
            {
                if (mode != BlendMode.Normal)
                {
                    // throws exception if corresponding Win2D BlendEffectMode is not present
                    Enum.Parse(typeof(BlendEffectMode), Enum.GetName(typeof(BlendMode), mode));
                }
            }
        }
    }
}
