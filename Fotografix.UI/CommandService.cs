using Fotografix.Editor.Commands;
using System;
using System.ComponentModel;

namespace Fotografix.UI
{
    /// <summary>
    /// Provides infrastructure for executing commands.
    /// </summary>
    public sealed class CommandService : NotifyPropertyChangedBase
    {
        private readonly History history;
        private bool busy;

        public CommandService()
        {
            this.history = new History();
            history.PropertyChanged += OnHistoryPropertyChanged;
        }

        public bool IsBusy
        {
            get => busy;
            private set => SetProperty(ref busy, value);
        }

        public bool CanUndo => history.CanUndo;
        public bool CanRedo => history.CanRedo;

        public void Execute(ICommand command)
        {
            Run(() =>
            {
                IChange change = command.PrepareChange();
                change.Apply();
                history.Add(change);
            });
        }

        public void Undo()
        {
            Run(history.Undo);
        }

        public void Redo()
        {
            Run(history.Redo);
        }

        public void AddChange(IChange change)
        {
            Run(() => history.Add(change));
        }

        private void Run(Action action)
        {
            if (busy)
            {
                throw new InvalidOperationException("Another operation is currently in progress");
            }

            this.IsBusy = true;

            try
            {
                action();
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void OnHistoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }
}
