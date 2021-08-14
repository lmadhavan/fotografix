using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public class WorkspaceCommandDispatcherTest
    {
        private Workspace workspace;
        private TestCommand editorCommand;

        private WorkspaceCommandDispatcher dispatcher;
        private AsyncCommand boundCommand;

        [SetUp]
        public void SetUp()
        {
            this.editorCommand = new();
            this.workspace = new();

            this.dispatcher = new(workspace);
            this.boundCommand = dispatcher.Bind(editorCommand);
        }

        [Test]
        public async Task BindsEditorCommandToWorkspace()
        {
            object parameter = new();
            editorCommand.Enabled = true;

            Assert.IsTrue(boundCommand.CanExecute(parameter));

            await boundCommand.ExecuteAsync(parameter);

            Assert.That(editorCommand.Workspace, Is.EqualTo(workspace));
            Assert.That(editorCommand.Parameter, Is.EqualTo(parameter));
        }

        [Test]
        public void RaisesCanExecuteChangedWhenActiveDocumentIsSet()
        {
            AssertCanExecuteChanged(() => workspace.ActiveDocument = new Document());
        }

        [Test]
        public void RaisesCanExecuteChangedWhenDocumentContentChanges()
        {
            Document document = new();
            document.Image.Size = new(10, 10);
            workspace.ActiveDocument = document;

            AssertCanExecuteChanged(() => document.Image.Size = new(20, 20));
        }

        [Test]
        public void IgnoresAdditionalCommandsWhenBusy()
        {
            editorCommand.BlockOnExit();

            Task first = boundCommand.ExecuteAsync();

            Assert.That(editorCommand.ExecuteCount, Is.EqualTo(1), "first command should trigger immediately");
            Assert.IsFalse(first.IsCompleted, "first task should not be complete");
            Assert.IsTrue(dispatcher.IsBusy, "dispatcher should be busy after triggering first command");

            Task second = boundCommand.ExecuteAsync();
            Assert.That(editorCommand.ExecuteCount, Is.EqualTo(1), "second command should be ignored");
            Assert.That(second.IsCompleted, "second task should be complete");
            Assert.IsTrue(dispatcher.IsBusy, "dispatcher should be busy after triggering second command");

            editorCommand.Unblock();

            Assert.IsTrue(first.IsCompleted, "first task should be complete");
            Assert.IsFalse(dispatcher.IsBusy, "dispatcher should not be busy after command completes");
        }

        [Test]
        public async Task CancelsActiveCommand()
        {
            editorCommand.BlockOnEntry();

            Task task = boundCommand.ExecuteAsync();

            dispatcher.CancelActiveCommand();
            editorCommand.Unblock();

            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                Assert.Pass();
            }

            Assert.Fail("expected cancellation exception");
        }

        private void AssertCanExecuteChanged(Action action)
        {
            bool canExecuteChanged = false;
            boundCommand.CanExecuteChanged += (s, e) => canExecuteChanged = true;

            action();

            Assert.IsTrue(canExecuteChanged);
        }

        private sealed class TestCommand : EditorCommand
        {
            private readonly TaskCompletionSource<int> tcs = new();
            private Task entryTask = Task.CompletedTask;
            private Task exitTask = Task.CompletedTask;

            public bool Enabled { get; set; }
            public int ExecuteCount { get; private set; }

            public Workspace Workspace { get; private set; }
            public object Parameter { get; private set; }

            public void BlockOnEntry()
            {
                this.entryTask = tcs.Task;
            }

            public void BlockOnExit()
            {
                this.exitTask = tcs.Task;
            }

            public void Unblock()
            {
                tcs.SetResult(0);
            }

            public override bool CanExecute(Workspace workspace, object parameter)
            {
                return Enabled;
            }

            public async override Task ExecuteAsync(Workspace workspace, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
            {
                await entryTask;

                this.Workspace = workspace;
                this.Parameter = parameter;
                cancellationToken.ThrowIfCancellationRequested();
                ExecuteCount++;

                await exitTask;
            }
        }
    }
}
