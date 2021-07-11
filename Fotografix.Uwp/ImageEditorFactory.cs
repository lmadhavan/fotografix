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

namespace Fotografix.Uwp
{
    public sealed class ImageEditorFactory
    {
        private readonly Workspace workspace;
        private readonly IClipboard clipboard;
        private readonly IImageDecoder imageDecoder = new WindowsImageDecoder();
        private readonly IImageEncoder imageEncoder = new WindowsImageEncoder(new Win2DImageRenderer());
        private readonly IGraphicsDevice graphicsDevice = new Win2DGraphicsDevice();
        private readonly FilePickerOverride filePickerOverride = new FilePickerOverride(new FilePickerAdapter());
        private readonly CommandHandlerCollection handlerCollection = new CommandHandlerCollection();

        public ImageEditorFactory(Workspace workspace, IClipboard clipboard)
        {
            this.workspace = workspace;
            this.clipboard = clipboard;

            this.NewCommand = workspace.Bind(new NewImageCommand(new ContentDialogAdapter<NewImageDialog, NewImageParameters>()));
            this.OpenCommand = workspace.Bind(new OpenImageCommand(imageDecoder, filePickerOverride));

            handlerCollection.Register(new DrawCommandHandler(graphicsDevice));
            handlerCollection.Register(new CropCommandHandler());
        }

        public IDialog<ResizeImageParameters> ResizeImageDialog { get; set; } = new ContentDialogAdapter<ResizeImageDialog, ResizeImageParameters>();
        public FilePickerOverride FilePickerOverride => filePickerOverride;

        public AsyncCommand NewCommand { get; }
        public AsyncCommand OpenCommand { get; }

        public ImageEditor CreateEditor(Viewport viewport, Document document)
        {
            DocumentCommandDispatcher dispatcher = new DocumentCommandDispatcher(document, handlerCollection);
            document.Image.SetCommandDispatcher(dispatcher);
            document.Image.SetViewport(viewport);

            var editor = new ImageEditor(document)
            {
                FilePickerOverride = filePickerOverride,
                Tools = CreateTools(),

                UndoCommand = workspace.Bind(new UndoCommand()),
                RedoCommand = workspace.Bind(new RedoCommand()),

                SaveCommand = workspace.Bind(new SaveImageCommand(imageEncoder, filePickerOverride) { Mode = SaveCommandMode.Save }),
                SaveAsCommand = workspace.Bind(new SaveImageCommand(imageEncoder, filePickerOverride) { Mode = SaveCommandMode.SaveAs }),
                PasteCommand = workspace.Bind(new PasteCommand(clipboard)),

                NewLayerCommand = workspace.Bind(new NewLayerCommand()),
                DeleteLayerCommand = workspace.Bind(new DeleteLayerCommand()),
                ImportLayerCommand = workspace.Bind(new ImportLayerCommand(imageDecoder, filePickerOverride)),

                ResizeImageCommand = workspace.Bind(new ResizeImageCommand(ResizeImageDialog, graphicsDevice))
            };

            return editor;
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
