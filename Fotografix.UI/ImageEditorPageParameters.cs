using Fotografix.UI.FileManagement;

namespace Fotografix.UI
{
    public sealed class ImageEditorPageParameters
    {
        public ImageEditorPageParameters(IWorkspace workspace, ICreateImageEditorCommand command)
        {
            this.Workspace = workspace;
            this.Command = command;
        }

        public IWorkspace Workspace { get; }
        public ICreateImageEditorCommand Command { get; }
    }
}
