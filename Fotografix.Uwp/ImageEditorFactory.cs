﻿using Fotografix.Editor;
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
            return new ImageEditor(image)
            {
                ImageDecoder = imageDecoder,
                ImageEncoder = imageEncoder,
                Tools = CreateTools(viewport)
            };
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
