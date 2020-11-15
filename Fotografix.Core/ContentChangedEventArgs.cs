using System;

namespace Fotografix
{
    public sealed class ContentChangedEventArgs : EventArgs
    {
        public ContentChangedEventArgs()
        {
        }

        public ContentChangedEventArgs(IChange change)
        {
            this.Change = change;
        }

        public IChange Change { get; }
    }
}