using System.Collections.Generic;

namespace Fotografix.Editor
{
    public sealed class History : NotifyPropertyChangedBase
    {
        private readonly Stack<Change> undoStack = new Stack<Change>();
        private readonly Stack<Change> redoStack = new Stack<Change>();

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        public void Add(Change change)
        {
            undoStack.Push(GetEffectiveChange(change));
            redoStack.Clear();
            RaiseEvents();
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

        private Change GetEffectiveChange(Change change)
        {
            if (undoStack.Count > 0 &&
                change.TryMergeWith(undoStack.Peek(), out Change result))
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