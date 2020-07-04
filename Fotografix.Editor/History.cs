using Fotografix.History;
using System.Collections.Generic;

namespace Fotografix.Editor
{
    public sealed class History : NotifyPropertyChangedBase, IChangeTracker
    {
        private readonly Stack<Change> undoStack = new Stack<Change>();
        private readonly Stack<Change> redoStack = new Stack<Change>();

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        public void Add(Change change)
        {
            if (!TryMergeIntoPreviousCommand(change))
            {
                undoStack.Push(change);
            }

            redoStack.Clear();
            RaiseEvents();
        }

        private bool TryMergeIntoPreviousCommand(Change change)
        {
            return undoStack.Count > 0
                && change.TryMergeInto(undoStack.Peek());
        }

        public void Undo()
        {
            Change change = undoStack.Pop();
            change.Undo();
            redoStack.Push(change);
            RaiseEvents();
        }

        public void Redo()
        {
            Change change = redoStack.Pop();
            change.Redo();
            undoStack.Push(change);
            RaiseEvents();
        }

        private void RaiseEvents()
        {
            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
        }
    }
}