using System;

namespace Fotografix
{
    public sealed class UserProperty<T>
    {
        public string Id { get; } = Guid.NewGuid().ToString();
    }
}
