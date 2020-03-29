using System.Collections.Generic;
using System.Linq;

namespace Fotografix.Editor
{
    public sealed class CompositeCommand : Command
    {
        private readonly IReadOnlyList<Command> commands;

        public CompositeCommand(IReadOnlyList<Command> commands)
        {
            this.commands = commands;
        }

        public CompositeCommand(params Command[] commands)
        {
            this.commands = commands;
        }

        public override bool IsEffective => commands.Any(c => c.IsEffective);

        public override void Execute()
        {
            foreach (Command command in commands)
            {
                command.Execute();
            }
        }

        public override void Undo()
        {
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                commands[i].Undo();
            }
        }
    }
}