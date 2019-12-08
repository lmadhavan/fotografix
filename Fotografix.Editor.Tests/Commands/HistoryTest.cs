using Fotografix.Editor.Commands;
using NUnit.Framework;

namespace Fotografix.Editor.Tests.Commands
{
    [TestFixture]
    public class HistoryTest
    {
        [Test]
        public void UndoesAndRedoesChange()
        {
            History history = new History();
            FakeChange change = new FakeChange();

            AssertHistoryState(history, canUndo: false, canRedo: false);

            history.Add(change);

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertCommandState(change, undoCount: 0, redoCount: 0);

            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);
            AssertCommandState(change, undoCount: 1, redoCount: 0);

            history.Redo();

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertCommandState(change, undoCount: 1, redoCount: 1);
        }

        [Test]
        public void AddingNewChangeClearsRedoStack()
        {
            History history = new History();

            history.Add(new FakeChange());
            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);

            history.Add(new FakeChange());

            AssertHistoryState(history, canUndo: true, canRedo: false);
        }

        private void AssertHistoryState(History history, bool canUndo, bool canRedo)
        {
            Assert.AreEqual(canUndo, history.CanUndo, "CanUndo");
            Assert.AreEqual(canRedo, history.CanRedo, "CanRedo");
        }

        private void AssertCommandState(FakeChange change, int undoCount, int redoCount)
        {
            Assert.AreEqual(undoCount, change.UndoCount, "UndoCount");
            Assert.AreEqual(redoCount, change.ApplyCount, "RedoCount");
        }

        private sealed class FakeChange : IChange
        {
            public int ApplyCount { get; private set; }
            public int UndoCount { get; private set; }

            public void Apply()
            {
                ApplyCount++;
            }

            public void Undo()
            {
                UndoCount++;
            }
        }
    }
}
