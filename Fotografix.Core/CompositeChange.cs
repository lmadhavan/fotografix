using System.Collections.Generic;

namespace Fotografix
{
    public sealed class CompositeChange : Change
    {
        private readonly IReadOnlyList<Change> changes;

        public CompositeChange(IReadOnlyList<Change> changes)
        {
            this.changes = changes;
        }

        public CompositeChange(params Change[] changes)
        {
            this.changes = changes;
        }

        public override void Undo()
        {
            for (int i = changes.Count - 1; i >= 0; i--)
            {
                changes[i].Undo();
            }
        }

        public override void Redo()
        {
            for (int i = 0; i < changes.Count; i++)
            {
                changes[i].Redo();
            }
        }
    }
}
