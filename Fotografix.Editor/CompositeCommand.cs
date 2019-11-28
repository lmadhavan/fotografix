using System.Collections.Generic;

namespace Fotografix.Editor
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

        public void Execute()
        {
            foreach (ICommand command in commands)
            {
                command.Execute();
            }
        }

        public void Undo()
        {
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                commands[i].Undo();
            }
        }

        public void Redo()
        {
            foreach (ICommand command in commands)
            {
                command.Redo();
            }
        }
    }
}