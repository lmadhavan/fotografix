using Fotografix.Editor;
using System;

namespace Fotografix.Uwp.FileManagement
{
    public delegate ImageEditor CreateImageEditorFunc(Viewport viewport);

    public sealed class OpenImageEditorRequestedEventArgs : EventArgs
    {
        public OpenImageEditorRequestedEventArgs(CreateImageEditorFunc createFunc)
        {
            this.CreateFunc = createFunc;
        }

        public CreateImageEditorFunc CreateFunc { get; }
    }
}