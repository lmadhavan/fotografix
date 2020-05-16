using Fotografix.UI;
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
    }
}