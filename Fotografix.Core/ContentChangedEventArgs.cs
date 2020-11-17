using System;

namespace Fotografix
{
    public sealed class ContentChangedEventArgs : EventArgs
    {
        public ContentChangedEventArgs()
        {
        }

        public ContentChangedEventArgs(Change change)
        {
            this.Change = change;
        }

        public Change Change { get; }
    }
}