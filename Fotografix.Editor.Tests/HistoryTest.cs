using NUnit.Framework;

namespace Fotografix.Editor
{
    [TestFixture]
    public class HistoryTest
    {
        private History history;

        [SetUp]
        public void SetUp()
        {
            this.history = new History();
        }

        [Test]
        public void UndoesAndRedoesCommand()
        {
            FakeCommand command = new FakeCommand();

            AssertHistoryState(history, canUndo: false, canRedo: false);

            history.Add(command);

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertCommandState(command, undoCount: 0, executeCount: 0);

            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);
            AssertCommandState(command, undoCount: 1, executeCount: 0);

            history.Redo();

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertCommandState(command, undoCount: 1, executeCount: 1);
        }

        [Test]
        public void AddingNewCommandClearsRedoStack()
        {
            history.Add(new FakeCommand());
            history.Undo();
            history.Add(new FakeCommand());

            AssertHistoryState(history, canUndo: true, canRedo: false);
        }

        [Test]
        public void MergesNewCommandIntoExistingCommandIfPossible()
        {
            FakeCommand existingCommand = new FakeCommand();
            history.Add(existingCommand);

            FakeCommand newCommand = new FakeCommand() { CanMerge = true };
            history.Add(newCommand);

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertCommandState(existingCommand, undoCount: 0, executeCount: 0, mergeCount: 1);

            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);
            AssertCommandState(existingCommand, undoCount: 1, executeCount: 0, mergeCount: 1);
            AssertCommandState(newCommand, undoCount: 0, executeCount: 0, mergeCount: 0);
        }

        private void AssertHistoryState(History history, bool canUndo, bool canRedo)
        {
            Assert.AreEqual(canUndo, history.CanUndo, "CanUndo");
            Assert.AreEqual(canRedo, history.CanRedo, "CanRedo");
        }

        private void AssertCommandState(FakeCommand command, int undoCount, int executeCount, int mergeCount = 0)
        {
            Assert.AreEqual(undoCount, command.UndoCount, "UndoCount");
            Assert.AreEqual(executeCount, command.ExecuteCount, "ExecuteCount");
            Assert.AreEqual(mergeCount, command.MergeCount, "MergeCount");
        }

        private sealed class FakeCommand : Command
        {
            public int ExecuteCount { get; private set; }
            public int UndoCount { get; private set; }
            public int MergeCount { get; private set; }
            public bool CanMerge { get; set; }

            public override void Execute()
            {
                ExecuteCount++;
            }

            public override void Undo()
            {
                UndoCount++;
            }

            public override bool TryMergeInto(Command command)
            {
                if (CanMerge)
                {
                    ((FakeCommand)command).MergeCount++;
                    return true;
                }

                return false;
            }
        }
    }
}
