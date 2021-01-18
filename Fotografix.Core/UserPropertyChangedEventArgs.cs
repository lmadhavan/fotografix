using System;

namespace Fotografix
{
    public sealed class UserPropertyChangedEventArgs : EventArgs
    {
        public UserPropertyChangedEventArgs(IUserPropertyKey key)
        {
            this.Key = key;
        }

        public IUserPropertyKey Key { get; }
    }
}