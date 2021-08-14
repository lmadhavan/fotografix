using Fotografix.Editor;
using Fotografix.Editor.ChangeTracking;
using Fotografix.Editor.Clipboard;
using Fotografix.Editor.FileManagement;
using Fotografix.Editor.Layers;
using Fotografix.Editor.Tools;
using Fotografix.IO;
using Fotografix.Uwp.Codecs;
using Fotografix.Uwp.FileManagement;
using Fotografix.Win2D;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Uwp
{
    public sealed class WorkspaceViewModel : NotifyPropertyChangedBase, IStartPageViewModel, IToolbox
    {
        private readonly Workspace workspace;
        private readonly FilePickerOverride filePickerOverride = new FilePickerOverride(new FilePickerAdapter());

        public WorkspaceViewModel(Workspace workspace, IClipboard clipboard, IDialog<ResizeImageParameters> resizeImageDialog)
        {
            this.workspace = workspace;
            workspace.DocumentAdded += Workspace_DocumentAdded;
            workspace.DocumentRemoved += Workspace_DocumentRemoved;
            workspace.PropertyChanged += Workspace_PropertyChanged;

            var imageDecoder = new WindowsImageDecoder();
            var imageEncoder = new WindowsImageEncoder(new Win2DImageRenderer());
            var graphicsDevice = new Win2DGraphicsDevice();

            var drawCommand = workspace.Bind(new DrawCommand(graphicsDevice));
            var cropCommand = workspace.Bind(new CropCommand());

            this.Tools = new List<ITool>
            {
                new HandTool(),
                new MoveTool(),
                new SelectionTool(),
                new CropTool(cropCommand),
                new BrushTool(drawCommand) { Size = 5, Color = Color.White },
                new GradientTool(drawCommand) { StartColor = Color.Black, EndColor = Color.White }
            };
            this.ActiveTool = Tools.First();

            this.NewCommand = workspace.Bind(new NewImageCommand(new ContentDialogAdapter<NewImageDialog, NewImageParameters>()));
            this.OpenCommand = workspace.Bind(new OpenImageCommand(imageDecoder, filePickerOverride));
            this.SaveCommand = workspace.Bind(new SaveImageCommand(imageEncoder, filePickerOverride) { Mode = SaveCommandMode.Save });
            this.SaveAsCommand = workspace.Bind(new SaveImageCommand(imageEncoder, filePickerOverride) { Mode = SaveCommandMode.SaveAs });

            this.UndoCommand = workspace.Bind(new UndoCommand());
            this.RedoCommand = workspace.Bind(new RedoCommand());
            this.PasteCommand = workspace.Bind(new PasteCommand(clipboard));

            this.ResizeImageCommand = workspace.Bind(new ResizeImageCommand(resizeImageDialog, graphicsDevice));

            this.NewLayerCommand = workspace.Bind(new NewLayerCommand());
            this.NewAdjustmentLayerCommand = workspace.Bind(new NewAdjustmentLayerCommand());
            this.DeleteLayerCommand = workspace.Bind(new DeleteLayerCommand());
            this.ImportLayerCommand = workspace.Bind(new ImportLayerCommand(imageDecoder, filePickerOverride));
        }

        #region Tools

        public IList<ITool> Tools { get; }

        public ITool ActiveTool
        {
            get => workspace.ActiveTool;
            set => workspace.ActiveTool = value;
        }

        #endregion

        #region Commands

        public AsyncCommand NewCommand { get; }
        public AsyncCommand OpenCommand { get; }
        public AsyncCommand SaveCommand { get; }
        public AsyncCommand SaveAsCommand { get; }

        public AsyncCommand UndoCommand { get; }
        public AsyncCommand RedoCommand { get; }
        public AsyncCommand PasteCommand { get; }

        public AsyncCommand ResizeImageCommand { get; }

        public AsyncCommand NewLayerCommand { get; }
        public AsyncCommand NewAdjustmentLayerCommand { get; }
        public AsyncCommand DeleteLayerCommand { get; }
        public AsyncCommand ImportLayerCommand { get; }

        #endregion

        #region Recent files

        public RecentFileList RecentFiles { get; } = RecentFileList.Default;

        public async Task OpenRecentFileAsync(RecentFile recentFile)
        {
            StorageFile storageFile = await RecentFiles.GetFileAsync(recentFile);
            await OpenFileAsync(new StorageFileAdapter(storageFile));
        }

        #endregion

        #region Open documents

        private readonly Dictionary<Document, ImageEditor> documentViewModels = new Dictionary<Document, ImageEditor>();
        private ImageEditor activeDocument;

        public ImageEditor ActiveDocument
        {
            get => activeDocument;
            private set => SetProperty(ref activeDocument, value);
        }

        public ImageEditor ViewModelFor(Document document)
        {
            return documentViewModels[document];
        }

        private void AddDocument(Document document)
        {
            var vm = CreateEditor(document);
            documentViewModels[document] = vm;
        }

        private void RemoveDocument(Document document)
        {
            if (documentViewModels.Remove(document, out var vm))
            {
                vm.Dispose();
            }
        }

        #endregion

        public async Task OpenFileAsync(IFile file)
        {
            using (filePickerOverride.OverrideOpenFile(file))
            {
                await OpenCommand.ExecuteAsync();
            }
        }

        private ImageEditor CreateEditor(Document document)
        {
            var editor = new ImageEditor(document)
            {
                Toolbox = this,
                FilePickerOverride = filePickerOverride,
                ImportLayerCommand = ImportLayerCommand
            };

            return editor;
        }

        private void Workspace_DocumentAdded(object sender, DocumentEventArgs e)
        {
            AddDocument(e.Document);
        }

        private void Workspace_DocumentRemoved(object sender, DocumentEventArgs e)
        {
            RemoveDocument(e.Document);
        }

        private void Workspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Workspace.ActiveDocument):
                    this.ActiveDocument = workspace.ActiveDocument == null ? null : ViewModelFor(workspace.ActiveDocument);
                    break;

                case nameof(Workspace.ActiveTool):
                    RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }
    }
}
