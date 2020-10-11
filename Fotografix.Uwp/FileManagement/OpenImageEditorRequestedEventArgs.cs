using System;

namespace Fotografix.Uwp.FileManagement
{
    public sealed class OpenImageEditorRequestedEventArgs : EventArgs
    {
        public OpenImageEditorRequestedEventArgs(ICreateImageEditorCommand command)
        {
            this.Command = command;
        }

        public ICreateImageEditorCommand Command { get; }
    }
}