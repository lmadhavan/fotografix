using System;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class Dialog<T> : IDialog<T>
    {
        private readonly Func<T, bool> func;

        public Dialog(Func<T, bool> func)
        {
            this.func = func;
        }

        public Task<bool> ShowAsync(T parameters)
        {
            return Task.FromResult(func(parameters));
        }
    }
}
