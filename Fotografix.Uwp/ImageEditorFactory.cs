using Fotografix.Editor;
using Fotografix.Editor.Commands;
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
        private readonly CommandHandlerCollection handlerCollection = new CommandHandlerCollection();

        public ImageEditorFactory()
        {
            var graphicsDevice = new Win2DGraphicsDevice();
            handlerCollection.Register(new ResampleImageCommandHandler(graphicsDevice));
            handlerCollection.Register(new DrawCommandHandler(graphicsDevice));
            handlerCollection.Register(new CropCommandHandler());

            SaveCommandHandler saveHandler = new SaveCommandHandler(imageEncoder, new FilePickerAdapter());
            handlerCollection.Register<SaveCommand>(saveHandler);
            handlerCollection.Register<SaveAsCommand>(saveHandler);
        }

        public IEnumerable<FileFormat> SupportedOpenFormats => imageDecoder.SupportedFileFormats;

        public ImageEditor CreateNewImage(Viewport viewport, Size size)
        {
            Layer layer = ImageEditor.CreateLayer(id: 1);
            Image image = new Image(size);
            image.Layers.Add(layer);

            return CreateEditor(viewport, image);
        }

        public async Task<ImageEditor> OpenImageAsync(Viewport viewport, IFile file)
        {
            Image image = await imageDecoder.ReadImageAsync(file);
            image.SetFile(file);
            return CreateEditor(viewport, image);
        }

        private ImageEditor CreateEditor(Viewport viewport, Image image)
        {
            Document document = new Document(image);
            DocumentCommandDispatcher dispatcher = new DocumentCommandDispatcher(document, handlerCollection);
            image.SetCommandDispatcher(dispatcher);
            image.SetViewport(viewport);

            var editor = new ImageEditor(document, dispatcher)
            {
                ImageDecoder = imageDecoder,
                Tools = CreateTools()
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
