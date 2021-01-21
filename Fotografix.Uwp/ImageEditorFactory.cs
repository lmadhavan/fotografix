﻿using Fotografix.Editor;
using Fotografix.Editor.Drawing;
using Fotografix.Editor.Tools;
using Fotografix.IO;
using Fotografix.Uwp.Codecs;
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
            handlerCollection.Register(new ResampleImageCommandHandler(new Win2DBitmapResamplingStrategy()));
            handlerCollection.Register(new DrawCommandHandler(new Win2DDrawingContextFactory()));
        }

        public IEnumerable<FileFormat> SupportedOpenFormats => imageDecoder.SupportedFileFormats;

        public ImageEditor CreateNewImage(Viewport viewport, Size size)
        {
            BitmapLayer layer = BitmapLayerFactory.CreateBitmapLayer(id: 1);
            Image image = new Image(size);
            image.Layers.Add(layer);

            return CreateEditor(viewport, image);
        }

        public async Task<ImageEditor> OpenImageAsync(Viewport viewport, IFile file)
        {
            Image image = await imageDecoder.ReadImageAsync(file);
            return CreateEditor(viewport, image);
        }

        private ImageEditor CreateEditor(Viewport viewport, Image image)
        {
            var editor = new ImageEditor(image, handlerCollection)
            {
                ImageDecoder = imageDecoder,
                ImageEncoder = imageEncoder,
                Tools = CreateTools(viewport)
            };
            image.SetCommandDispatcher(editor);
            return editor;
        }

        private IList<ITool> CreateTools(Viewport viewport)
        {
            return new List<ITool>
            {
                new HandTool(viewport),
                new BrushTool() { Size = 5, Color = Color.White },
                new GradientTool { StartColor = Color.Black, EndColor = Color.White }
            };
        }
    }
}
