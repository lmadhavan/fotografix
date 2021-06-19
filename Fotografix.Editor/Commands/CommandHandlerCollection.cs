using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Editor.Commands
{
    public sealed class CommandHandlerCollection : ICommandDispatcher
    {
        private readonly Dictionary<Type, object> handlers = new Dictionary<Type, object>();

        public void Register<T>(ICommandHandler<T> handler)
        {
            handlers[typeof(T)] = handler;
        }

        public Task DispatchAsync<T>(T command)
        {
            Type type = command.GetType();

            if (handlers.TryGetValue(type, out object handler))
            {
                return ((ICommandHandler<T>)handler).HandleAsync(command);
            }
            else
            {
                throw new InvalidOperationException("No handler registered for " + type.Name);
            }
        }
    }
}
