using Fotografix.Editor;
using Fotografix.Editor.ChangeTracking;
using Fotografix.Editor.Clipboard;
using Fotografix.Editor.Commands;
using Fotografix.Editor.FileManagement;
using Fotografix.Editor.Layers;
using Fotografix.Editor.Tools;
using Fotografix.IO;
using Fotografix.Uwp.Codecs;
using Fotografix.Uwp.FileManagement;
using Fotografix.Win2D;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Uwp
{
    public sealed class WorkspaceViewModel : IStartPageViewModel
    {
        private readonly Workspace workspace;
        private readonly IImageDecoder imageDecoder = new WindowsImageDecoder();
        private readonly IImageEncoder imageEncoder = new WindowsImageEncoder(new Win2DImageRenderer());
        private readonly IGraphicsDevice graphicsDevice = new Win2DGraphicsDevice();
        private readonly FilePickerOverride filePickerOverride = new FilePickerOverride(new FilePickerAdapter());
        private readonly CommandHandlerCollection handlerCollection = new CommandHandlerCollection();

        public WorkspaceViewModel(Workspace workspace, IClipboard clipboard, IDialog<ResizeImageParameters> resizeImageDialog)
        {
            this.workspace = workspace;

            this.NewCommand = workspace.Bind(new NewImageCommand(new ContentDialogAdapter<NewImageDialog, NewImageParameters>()));
            this.OpenCommand = workspace.Bind(new OpenImageCommand(imageDecoder, filePickerOverride));
            this.SaveCommand = workspace.Bind(new SaveImageCommand(imageEncoder, filePickerOverride) { Mode = SaveCommandMode.Save });
            this.SaveAsCommand = workspace.Bind(new SaveImageCommand(imageEncoder, filePickerOverride) { Mode = SaveCommandMode.SaveAs });

            this.UndoCommand = workspace.Bind(new UndoCommand());
            this.RedoCommand = workspace.Bind(new RedoCommand());
            this.PasteCommand = workspace.Bind(new PasteCommand(clipboard));

            this.ResizeImageCommand = workspace.Bind(new ResizeImageCommand(resizeImageDialog, graphicsDevice));

            this.NewLayerCommand = workspace.Bind(new NewLayerCommand());
            this.DeleteLayerCommand = workspace.Bind(new DeleteLayerCommand());
            this.ImportLayerCommand = workspace.Bind(new ImportLayerCommand(imageDecoder, filePickerOverride));

            handlerCollection.Register(new DrawCommandHandler(graphicsDevice));
            handlerCollection.Register(new CropCommandHandler());
        }

        public RecentFileList RecentFiles { get; } = RecentFileList.Default;

        public AsyncCommand NewCommand { get; }
        public AsyncCommand OpenCommand { get; }
        public AsyncCommand SaveCommand { get; }
        public AsyncCommand SaveAsCommand { get; }

        public AsyncCommand UndoCommand { get; }
        public AsyncCommand RedoCommand { get; }
        public AsyncCommand PasteCommand { get; }

        public AsyncCommand ResizeImageCommand { get; }

        public AsyncCommand NewLayerCommand { get; }
        public AsyncCommand DeleteLayerCommand { get; }
        public AsyncCommand ImportLayerCommand { get; }

        public ImageEditor CreateEditor(Viewport viewport, Document document)
        {
            DocumentCommandDispatcher dispatcher = new DocumentCommandDispatcher(document, handlerCollection);
            document.Image.SetCommandDispatcher(dispatcher);
            document.Image.SetViewport(viewport);

            var editor = new ImageEditor(document)
            {
                FilePickerOverride = filePickerOverride,
                Tools = CreateTools(),
                NewLayerCommand = NewLayerCommand,
                DeleteLayerCommand = DeleteLayerCommand,
                ImportLayerCommand = ImportLayerCommand,
                ResizeImageCommand = ResizeImageCommand
            };

            return editor;
        }

        public async Task<ImageEditor> CreateEditorAsync(Viewport viewport, IFile file)
        {
            await OpenFileAsync(file);
            return CreateEditor(viewport, workspace.ActiveDocument);
        }

        public async Task OpenRecentFileAsync(RecentFile recentFile)
        {
            StorageFile storageFile = await RecentFiles.GetFileAsync(recentFile);
            await OpenFileAsync(new StorageFileAdapter(storageFile));
        }

        private async Task OpenFileAsync(IFile file)
        {
            using (filePickerOverride.OverrideOpenFile(file))
            {
                await OpenCommand.ExecuteAsync();
            }
        }

        private IList<ITool> CreateTools()
        {
            return new List<ITool>
            {
                new HandTool(),
                new MoveTool(),
                new SelectionTool(),
                new CropTool(),
                new BrushTool() { Size = 5, Color = Color.White },
                new GradientTool { StartColor = Color.Black, EndColor = Color.White }
            };
        }
    }
}
