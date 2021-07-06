﻿using Fotografix.Editor;
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

namespace Fotografix.Uwp
{
    public sealed class ImageEditorFactory
    {
        private readonly IImageDecoder imageDecoder = new WindowsImageDecoder();
        private readonly IImageEncoder imageEncoder = new WindowsImageEncoder(new Win2DImageRenderer());
        private readonly IFilePicker filePicker = new FilePickerAdapter();
        private readonly CommandHandlerCollection handlerCollection = new CommandHandlerCollection();
        private readonly IClipboard clipboard;

        public ImageEditorFactory(IClipboard clipboard)
        {
            var graphicsDevice = new Win2DGraphicsDevice();
            handlerCollection.Register(new ResampleImageCommandHandler(graphicsDevice));
            handlerCollection.Register(new DrawCommandHandler(graphicsDevice));
            handlerCollection.Register(new CropCommandHandler());

            this.clipboard = clipboard;
        }

        public IEnumerable<FileFormat> SupportedOpenFormats => imageDecoder.SupportedFileFormats;

        public ImageEditor CreateNewImage(Viewport viewport, Size size)
        {
            Layer layer = ImageEditor.CreateLayer(id: 1);
            Image image = new Image(size);
            image.Layers.Add(layer);

            return CreateEditor(viewport, new Document(image));
        }

        public async Task<ImageEditor> OpenImageAsync(Viewport viewport, IFile file)
        {
            Image image = await imageDecoder.ReadImageAsync(file);
            return CreateEditor(viewport, new Document(image) { File = file });
        }

        private ImageEditor CreateEditor(Viewport viewport, Document document)
        {
            Workspace workspace = new Workspace { ActiveDocument = document };

            DocumentCommandDispatcher dispatcher = new DocumentCommandDispatcher(document, handlerCollection);
            document.Image.SetCommandDispatcher(dispatcher);
            document.Image.SetViewport(viewport);

            FilePickerOverride filePickerOverride = new FilePickerOverride(filePicker);

            var editor = new ImageEditor(document, dispatcher)
            {
                FilePickerOverride = filePickerOverride,
                Tools = CreateTools(),

                SaveCommand = workspace.Bind(new SaveCommand(imageEncoder, filePicker) { Mode = SaveCommandMode.Save }),
                SaveAsCommand = workspace.Bind(new SaveCommand(imageEncoder, filePicker) { Mode = SaveCommandMode.SaveAs }),
                PasteCommand = workspace.Bind(new PasteCommand(clipboard)),

                NewLayerCommand = workspace.Bind(new NewLayerCommand()),
                DeleteLayerCommand = workspace.Bind(new DeleteLayerCommand()),
                ImportLayerCommand = workspace.Bind(new ImportLayerCommand(imageDecoder, filePickerOverride))
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
