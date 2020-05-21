using System;

namespace Fotografix
{
    public interface INotifyContentChanged
    {
        event EventHandler<ContentChangedEventArgs> ContentChanged;
    }
}
