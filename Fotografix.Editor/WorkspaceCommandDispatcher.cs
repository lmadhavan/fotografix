using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class WorkspaceCommandDispatcher : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Workspace workspace;
        private Document activeDocument;

        private readonly Progress<EditorCommandProgress> progress = new();
        private bool busy;
        private CancellationTokenSource cts;

        public WorkspaceCommandDispatcher(Workspace workspace)
        {
            this.workspace = workspace;
            workspace.PropertyChanged += Workspace_PropertyChanged;
            UpdateActiveDocument();
        }

        public void Dispose()
        {
            workspace.PropertyChanged -= Workspace_PropertyChanged;
        }

        public bool IsBusy
        {
            get => busy;
            private set => SetProperty(ref busy, value);
        }

        public event EventHandler<EditorCommandProgress> ProgressChanged
        {
            add => progress.ProgressChanged += value;
            remove => progress.ProgressChanged -= value;
        }

        public AsyncCommand Bind(EditorCommand command)
        {
            return new EditorCommandAdapter(command, this);
        }

        public void CancelActiveCommand()
        {
            cts?.Cancel();
        }

        public bool CanExecute(EditorCommand command, object parameter)
        {
            return command.CanExecute(workspace, parameter);
        }

        public async Task ExecuteAsync(EditorCommand command, object parameter)
        {
            if (busy)
            {
                Debug.WriteLine($"Ignoring {command.GetType().Name} because previous command has not completed yet");
                return;
            }

            try
            {
                this.cts = new();
                this.IsBusy = true;

                Debug.WriteLine($"Executing {command.GetType().Name} with parameter [{parameter}]");
                await command.ExecuteAsync(workspace, parameter, cts.Token, progress);
            }
            finally
            {
                cts.Dispose();
                this.cts = null;
                this.IsBusy = false;
            }
        }

        private event EventHandler RequerySuggested;

        private void UpdateActiveDocument()
        {
            if (activeDocument != null)
            {
                activeDocument.ContentChanged -= ActiveDocument_ContentChanged;
            }

            this.activeDocument = workspace.ActiveDocument;

            if (activeDocument != null)
            {
                activeDocument.ContentChanged += ActiveDocument_ContentChanged;
            }
        }

        private void Workspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Workspace.ActiveDocument))
            {
                UpdateActiveDocument();
                RequerySuggested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ActiveDocument_ContentChanged(object sender, EventArgs e)
        {
            RequerySuggested?.Invoke(this, EventArgs.Empty);
        }

        private sealed class EditorCommandAdapter : AsyncCommand
        {
            private readonly EditorCommand editorCommand;
            private readonly WorkspaceCommandDispatcher dispatcher;

            public EditorCommandAdapter(EditorCommand editorCommand, WorkspaceCommandDispatcher dispatcher)
            {
                this.editorCommand = editorCommand;
                this.dispatcher = dispatcher;
            }

            public override event EventHandler CanExecuteChanged
            {
                add => dispatcher.RequerySuggested += value;
                remove => dispatcher.RequerySuggested -= value;
            }

            public override bool CanExecute(object parameter)
            {
                return dispatcher.CanExecute(editorCommand, parameter);
            }

            public override Task ExecuteAsync(object parameter)
            {
                return dispatcher.ExecuteAsync(editorCommand, parameter);
            }
        }
    }
}
