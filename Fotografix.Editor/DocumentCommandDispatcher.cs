using Fotografix.Editor.Commands;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class DocumentCommandDispatcher : ICommandDispatcher
    {
        private readonly Document document;
        private readonly ICommandDispatcher dispatcher;

        public DocumentCommandDispatcher(Document document, ICommandDispatcher dispatcher)
        {
            this.document = document;
            this.dispatcher = dispatcher;
        }

        public async Task DispatchAsync<T>(T command)
        {
            using (document.BeginChangeGroup())
            {
                await dispatcher.DispatchAsync(command);
            }
        }
    }
}
