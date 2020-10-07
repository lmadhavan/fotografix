﻿using Fotografix.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.UI
{
    public sealed class StorageFileAdapter : IFile
    {
        private readonly StorageFile file;

        public StorageFileAdapter(StorageFile file)
        {
            this.file = file;
        }

        public string Name => file.Name;

        public async Task<Stream> OpenReadAsync()
        {
            var randomAccessStream = await file.OpenReadAsync();
            return randomAccessStream.AsStream();
        }

        public async Task<Stream> OpenWriteAsync()
        {
            var randomAccessStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            return randomAccessStream.AsStream();
        }
    }
}
