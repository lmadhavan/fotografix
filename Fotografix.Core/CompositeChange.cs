using System.Collections.Generic;

namespace Fotografix
{
    public sealed class CompositeChange : IChange
    {
        private readonly IReadOnlyList<IChange> changes;

        public CompositeChange(IReadOnlyList<IChange> changes)
        {
            this.changes = changes;
        }

        public CompositeChange(params IChange[] changes)
        {
            this.changes = changes;
        }

        public void Undo()
        {
            for (int i = changes.Count - 1; i >= 0; i--)
            {
                changes[i].Undo();
            }
        }

        public void Redo()
        {
            for (int i = 0; i < changes.Count; i++)
            {
                changes[i].Redo();
            }
        }
    }
}
