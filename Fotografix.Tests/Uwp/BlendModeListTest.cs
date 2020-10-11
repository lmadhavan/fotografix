using Fotografix.Uwp.BlendModes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class BlendModeListTest
    {
        private BlendModeList list;
        
        [TestInitialize]
        public void Initialize()
        {
            this.list = BlendModeList.Create();
        }

        [TestMethod]
        public void MapsEnumValueToDisplayName()
        {
            Assert.AreEqual("Soft Light", list[BlendMode.SoftLight].Name);
        }

        [TestMethod]
        public void AddsSeparatorBetweenGroups()
        {
            BlendMode aGroupMarker = BlendModeList.GroupMarkers.First();
            int index = list.IndexOf(aGroupMarker);

            Assert.IsTrue(list[index - 1].IsSeparator, "Item before first grouped item must be a separator");
        }
    }
}
