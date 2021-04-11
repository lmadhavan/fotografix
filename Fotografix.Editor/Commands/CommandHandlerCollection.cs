using System;
using System.Collections.Generic;

namespace Fotografix.Editor.Commands
{
    public sealed class CommandHandlerCollection : ICommandDispatcher
    {
        private readonly Dictionary<Type, object> handlers = new Dictionary<Type, object>();

        public void Register<T>(ICommandHandler<T> handler)
        {
            handlers[typeof(T)] = handler;
        }

        public void Dispatch<T>(T command)
        {
            Type type = command.GetType();

            if (handlers.TryGetValue(type, out object handler))
            {
                ((ICommandHandler<T>)handler).Handle(command);
            }
            else
            {
                throw new InvalidOperationException("No handler registered for " + type.Name);
            }
        }
    }
}
