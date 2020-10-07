﻿using Fotografix.Editor;
using Fotografix.IO;
using System.Threading.Tasks;

namespace Fotografix.UI.FileManagement
{
    public sealed class OpenFileCommand : ICreateImageEditorCommand
    {
        private readonly IFile file;
        private readonly IImageDecoder imageDecoder;
        private readonly IImageEncoder imageEncoder;

        public OpenFileCommand(IFile file, IImageDecoder imageDecoder, IImageEncoder imageEncoder)
        {
            this.file = file;
            this.imageDecoder = imageDecoder;
            this.imageEncoder = imageEncoder;
        }

        public string Title => file.Name;

        public async Task<ImageEditor> ExecuteAsync(Viewport viewport)
        {
            return await ImageEditor.CreateAsync(file, viewport, imageDecoder, imageEncoder);
        }
    }
}