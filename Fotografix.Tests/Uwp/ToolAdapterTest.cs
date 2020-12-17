using Fotografix.Editor.Tools;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class ToolAdapterTest
    {
        [TestMethod]
        public void ContainsMappingsForAllToolCursors()
        {
            foreach (ToolCursor toolCursor in Enum.GetValues(typeof(ToolCursor)))
            {
                Assert.IsTrue(ToolAdapter.CursorMap.ContainsKey(toolCursor), "No mapping found for " + toolCursor);
            }
        }
    }
}
