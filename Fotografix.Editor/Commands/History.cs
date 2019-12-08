using System.Collections.Generic;

namespace Fotografix.Editor.Commands
{
    public sealed class History : NotifyPropertyChangedBase
    {
        private readonly Stack<IChange> undoStack = new Stack<IChange>();
        private readonly Stack<IChange> redoStack = new Stack<IChange>();

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        public void Add(IChange change)
        {
            undoStack.Push(change);
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
            change.Apply();
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