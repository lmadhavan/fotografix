using System.Collections.Generic;
using System.Linq;

namespace Fotografix.Editor.Commands
{
    public sealed class CompositeCommand : ICommand
    {
        private readonly IReadOnlyList<ICommand> commands;

        public CompositeCommand(IReadOnlyList<ICommand> commands)
        {
            this.commands = commands;
        }

        public CompositeCommand(params ICommand[] commands)
        {
            this.commands = commands;
        }

        public IChange PrepareChange()
        {
            return new CompositeChange(commands.Select(c => c.PrepareChange()).ToList());
        }

        private sealed class CompositeChange : IChange
        {
            private readonly IReadOnlyList<IChange> changes;

            public CompositeChange(IReadOnlyList<IChange> changes)
            {
                this.changes = changes;
            }

            public void Apply()
            {
                foreach (IChange change in changes)
                {
                    change.Apply();
                }
            }

            public void Undo()
            {
                for (int i = changes.Count - 1; i >= 0; i--)
                {
                    changes[i].Undo();
                }
            }
        }
    }
}