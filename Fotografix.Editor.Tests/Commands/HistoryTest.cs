using Fotografix.Editor.Commands;
using NUnit.Framework;

namespace Fotografix.Editor.Tests.Commands
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
        public void UndoesAndRedoesChange()
        {
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
            history.Add(new FakeChange());
            history.Undo();
            history.Add(new FakeChange());

            AssertHistoryState(history, canUndo: true, canRedo: false);
        }

        [Test]
        public void MergesNewChangeIntoExistingChangeIfPossible()
        {
            FakeChange existingChange = new FakeChange();
            history.Add(existingChange);

            FakeChange newChange = new FakeChange() { CanMerge = true };
            history.Add(newChange);

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertCommandState(existingChange, undoCount: 0, redoCount: 0, mergeCount: 1);

            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);
            AssertCommandState(existingChange, undoCount: 1, redoCount: 0, mergeCount: 1);
            AssertCommandState(newChange, undoCount: 0, redoCount: 0, mergeCount: 0);
        }

        private void AssertHistoryState(History history, bool canUndo, bool canRedo)
        {
            Assert.AreEqual(canUndo, history.CanUndo, "CanUndo");
            Assert.AreEqual(canRedo, history.CanRedo, "CanRedo");
        }

        private void AssertCommandState(FakeChange change, int undoCount, int redoCount, int mergeCount = 0)
        {
            Assert.AreEqual(undoCount, change.UndoCount, "UndoCount");
            Assert.AreEqual(redoCount, change.ApplyCount, "RedoCount");
            Assert.AreEqual(mergeCount, change.MergeCount, "MergeCount");
        }

        private sealed class FakeChange : IMergeableChange
        {
            public int ApplyCount { get; private set; }
            public int UndoCount { get; private set; }
            public int MergeCount { get; private set; }
            public bool CanMerge { get; set; }

            public void Apply()
            {
                ApplyCount++;
            }

            public void Undo()
            {
                UndoCount++;
            }

            public bool TryMergeInto(IChange change)
            {
                if (CanMerge)
                {
                    ((FakeChange)change).MergeCount++;
                    return true;
                }

                return false;
            }
        }
    }
}
