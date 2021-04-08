using System.Collections.Generic;

namespace Fotografix.Editor
{
    public sealed class History : NotifyPropertyChangedBase
    {
        private readonly Stack<IChange> undoStack = new Stack<IChange>();
        private readonly Stack<IChange> redoStack = new Stack<IChange>();

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        public void Add(IChange change)
        {
            undoStack.Push(GetEffectiveChange(change));
            redoStack.Clear();
            RaiseEvents();
        }

        public void Undo()
        {
            IChange change = undoStack.Pop();
            change.Undo();
            redoStack.Push(change);
            RaiseEvents();
        }

        public void Redo()
        {
            IChange change = redoStack.Pop();
            change.Redo();
            undoStack.Push(change);
            RaiseEvents();
        }

        private IChange GetEffectiveChange(IChange change)
        {
            if (undoStack.Count > 0 &&
                change is IMergeableChange mergeable &&
                mergeable.TryMergeWith(undoStack.Peek(), out IChange result))
            {
                undoStack.Pop();
                return result;
            }

            return change;
        }

        private void RaiseEvents()
        {
            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
        }
    }
}