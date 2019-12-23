using Fotografix.Editor.Commands;
using Fotografix.UI.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fotografix.Tests.UI.Commands
{
    [TestClass]
    public class CommandServiceTest
    {
        private CommandService commandService;

        [TestInitialize]
        public void Initialize()
        {
            this.commandService = new CommandService();
        }

        [TestMethod]
        public void ExecutesCommand()
        {
            FakeCommand command = new FakeCommand();

            commandService.Execute(command);

            Assert.IsTrue(command.Prepared, "Command should have been prepared");
            Assert.IsTrue(command.Applied, "Change should have been applied");
            Assert.IsFalse(command.Undone, "Change should not have been undone");

            Assert.IsTrue(commandService.CanUndo, "Should be able to undo");
            Assert.IsFalse(commandService.CanRedo, "Should not be able to redo");
        }

        [TestMethod]
        public void RejectsExternalChangesWhileExecuting()
        {
            FakeCommand command = new FakeCommand();

            Assert.IsFalse(commandService.IsBusy, "Should not be busy initially");

            command.Action = AssertBusyAndTryAddChange;

            Assert.ThrowsException<InvalidOperationException>(() => commandService.Execute(command));

            Assert.IsFalse(commandService.IsBusy, "Should not be busy after executing command");
        }

        [TestMethod]
        public void RejectsExternalChangesWhileUndoing()
        {
            FakeCommand command = new FakeCommand();

            Assert.IsFalse(commandService.IsBusy, "Should not be busy initially");

            commandService.Execute(command);
            command.Action = AssertBusyAndTryAddChange;

            Assert.ThrowsException<InvalidOperationException>(() => commandService.Undo());

            Assert.IsFalse(commandService.IsBusy, "Should not be busy after undoing");
        }

        [TestMethod]
        public void RejectsExternalChangesWhileRedoing()
        {
            FakeCommand command = new FakeCommand();

            Assert.IsFalse(commandService.IsBusy, "Should not be busy initially");

            commandService.Execute(command);
            commandService.Undo();
            command.Action = AssertBusyAndTryAddChange;

            Assert.ThrowsException<InvalidOperationException>(() => commandService.Redo());

            Assert.IsFalse(commandService.IsBusy, "Should not be busy after redoing");
        }

        private void AssertBusyAndTryAddChange()
        {
            Assert.IsTrue(commandService.IsBusy, "Should be busy while processing");

            // the call below should trigger an exception since we are busy
            commandService.AddChange(new FakeCommand());
        }

        private sealed class FakeCommand : ICommand, IChange
        {
            public FakeCommand()
            {
                this.Action = () => { };
            }

            public Action Action { get; set; }

            public bool Prepared { get; private set; }
            public bool Applied { get; private set; }
            public bool Undone { get; private set; }

            public IChange PrepareChange()
            {
                Action();
                this.Prepared = true;
                return this;
            }

            void IChange.Apply()
            {
                Action();
                this.Applied = true;
            }

            void IChange.Undo()
            {
                Action();
                this.Undone = true;
            }
        }
    }
}
