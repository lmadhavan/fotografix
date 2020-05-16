using Fotografix.Adjustments;
using Fotografix.UI;
using Fotografix.UI.Adjustments;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    public abstract class AcceptanceTestBase
    {
        protected ImageEditor Editor { get; private set; }

        protected async Task OpenImageAsync(string filename)
        {
            Editor?.Dispose();

            var file = await TestImages.GetFileAsync(filename);
            this.Editor = await ImageEditor.CreateAsync(file);
        }

        protected async Task RenderToTempFolderAsync(string filename)
        {
            await TestRenderer.RenderToTempFolderAsync(Editor, filename);
        }

        protected async Task AssertImageAsync(string fileWithExpectedImage)
        {
            await AssertImage.IsEquivalentAsync(fileWithExpectedImage, Editor);
        }

        protected void AddAdjustmentLayer<T>() where T : Adjustment, new()
        {
            Editor.AddAdjustmentLayer(new AdjustmentLayerFactory<T>("Test Adjustment"));
        }
    }
}