using System.Collections.Generic;

namespace Fotografix.Editor
{
    public sealed class History : NotifyPropertyChangedBase
    {
        private readonly Stack<Command> undoStack = new Stack<Command>();
        private readonly Stack<Command> redoStack = new Stack<Command>();

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        public void Add(Command command)
        {
            if (!TryMergeIntoPreviousCommand(command))
            {
                undoStack.Push(command);
            }

            redoStack.Clear();
            RaiseEvents();
        }

        private bool TryMergeIntoPreviousCommand(Command command)
        {
            return undoStack.Count > 0
                && command.TryMergeInto(undoStack.Peek());
        }

        public void Undo()
        {
            Command command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
            RaiseEvents();
        }

        public void Redo()
        {
            Command command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
            RaiseEvents();
        }

        private void RaiseEvents()
        {
            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
        }
    }
}