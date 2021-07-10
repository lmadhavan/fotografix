using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    public sealed class NewImageCommand : IWorkspaceCommand
    {
        private readonly IDialog<NewImageParameters> newImageDialog;

        public NewImageCommand(IDialog<NewImageParameters> newImageDialog)
        {
            this.newImageDialog = newImageDialog;
        }

        public async Task ExecuteAsync(Workspace workspace)
        {
            NewImageParameters parameters = new();

            if (await newImageDialog.ShowAsync(parameters))
            {
                Image image = new Image(parameters.Size);
                image.Layers.Add(new Layer { Name = "Layer 1" });

                Document document = new Document(image);
                workspace.AddDocument(document);
                workspace.ActiveDocument = document;
            }
        }
    }
}
