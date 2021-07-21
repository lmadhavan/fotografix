using System;

namespace Fotografix
{
    public sealed class DisposableAction : IDisposable
    {
        private readonly Action disposeAction;

        public DisposableAction(Action disposeAction)
        {
            this.disposeAction = disposeAction;
        }

        public void Dispose()
        {
            disposeAction();
        }
    }
}
