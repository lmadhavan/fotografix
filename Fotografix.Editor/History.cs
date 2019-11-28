using System.Collections.Generic;

namespace Fotografix.Editor
{
    public sealed class History : NotifyPropertyChangedBase
    {
        private readonly Stack<ICommand> undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> redoStack = new Stack<ICommand>();

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        public void Add(ICommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();
            RaiseEvents();
        }

        public void Undo()
        {
            ICommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
            RaiseEvents();
        }

        public void Redo()
        {
            ICommand command = redoStack.Pop();
            command.Redo();
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