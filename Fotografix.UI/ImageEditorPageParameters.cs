using Windows.Storage;

namespace Fotografix.UI
{
    public sealed class ImageEditorPageParameters
    {
        public ImageEditorPageParameters(IWorkspace workspace, StorageFile file)
        {
            this.Workspace = workspace;
            this.File = file;
        }

        public IWorkspace Workspace { get; }
        public StorageFile File { get; }
    }
}
