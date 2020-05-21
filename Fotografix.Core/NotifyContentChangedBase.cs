using System;

namespace Fotografix
{
    public abstract class NotifyContentChangedBase : NotifyPropertyChangedBase, INotifyContentChanged
    {
        public event EventHandler<ContentChangedEventArgs> ContentChanged;

        protected void RaiseContentChanged()
        {
            ContentChanged?.Invoke(this, new ContentChangedEventArgs());
        }
    }
}
