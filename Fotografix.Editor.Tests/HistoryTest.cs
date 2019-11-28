using NUnit.Framework;

namespace Fotografix.Editor.Tests
{
    [TestFixture]
    public class HistoryTest
    {
        [Test]
        public void UndoesAndRedoesCommand()
        {
            History history = new History();
            FakeCommand command = new FakeCommand();

            AssertHistoryState(history, canUndo: false, canRedo: false);

            history.Add(command);

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertCommandState(command, undoCount: 0, redoCount: 0);

            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);
            AssertCommandState(command, undoCount: 1, redoCount: 0);

            history.Redo();

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertCommandState(command, undoCount: 1, redoCount: 1);
        }

        [Test]
        public void AddingNewCommandClearsRedoStack()
        {
            History history = new History();

            history.Add(new FakeCommand());
            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);

            history.Add(new FakeCommand());

            AssertHistoryState(history, canUndo: true, canRedo: false);
        }

        private void AssertHistoryState(History history, bool canUndo, bool canRedo)
        {
            Assert.AreEqual(canUndo, history.CanUndo, "CanUndo");
            Assert.AreEqual(canRedo, history.CanRedo, "CanRedo");
        }

        private void AssertCommandState(FakeCommand command, int undoCount, int redoCount)
        {
            Assert.AreEqual(undoCount, command.UndoCount, "UndoCount");
            Assert.AreEqual(redoCount, command.RedoCount, "RedoCount");
        }

        private sealed class FakeCommand : ICommand
        {
            public int UndoCount { get; private set; }
            public int RedoCount { get; private set; }

            public void Execute()
            {
                Assert.Fail("Execute should never be called by History");
            }

            public void Undo()
            {
                UndoCount++;
            }

            public void Redo()
            {
                RedoCount++;
            }
        }
    }
}
