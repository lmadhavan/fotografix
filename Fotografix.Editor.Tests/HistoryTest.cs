using NUnit.Framework;
using System.Collections.Generic;

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
        public void UndoesAndRedoesChange()
        {
            FakeChange change = new FakeChange();

            AssertHistoryState(history, canUndo: false, canRedo: false);

            history.Add(change);

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertChangeState(change, undoCount: 0, redoCount: 0);

            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);
            AssertChangeState(change, undoCount: 1, redoCount: 0);

            history.Redo();

            AssertHistoryState(history, canUndo: true, canRedo: false);
            AssertChangeState(change, undoCount: 1, redoCount: 1);
        }

        [Test]
        public void ClearsRedoStackWhenAddingNewChange()
        {
            history.Add(new FakeChange());
            history.Undo();
            history.Add(new FakeChange());

            AssertHistoryState(history, canUndo: true, canRedo: false);
        }

        [Test]
        public void MergesChangesIfPossible()
        {
            FakeChange existingChange = new FakeChange();
            FakeChange newChange = new FakeChange();
            FakeChange mergedChange = new FakeChange();
            newChange.MergeResult = mergedChange;

            history.Add(existingChange);
            history.Add(newChange);

            Assert.That(newChange.MergedChanges, Is.EqualTo(new List<Change> { existingChange }));

            history.Undo();

            AssertHistoryState(history, canUndo: false, canRedo: true);
            AssertChangeState(mergedChange, undoCount: 1, redoCount: 0);
            AssertChangeState(existingChange, undoCount: 0, redoCount: 0);
            AssertChangeState(newChange, undoCount: 0, redoCount: 0);
        }

        private void AssertHistoryState(History history, bool canUndo, bool canRedo)
        {
            Assert.AreEqual(canUndo, history.CanUndo, "CanUndo");
            Assert.AreEqual(canRedo, history.CanRedo, "CanRedo");
        }

        private void AssertChangeState(FakeChange change, int undoCount, int redoCount)
        {
            Assert.AreEqual(undoCount, change.UndoCount, "UndoCount");
            Assert.AreEqual(redoCount, change.RedoCount, "RedoCount");
        }

        private sealed class FakeChange : Change
        {
            public int UndoCount { get; private set; }
            public int RedoCount { get; private set; }

            public List<Change> MergedChanges { get; } = new List<Change>();
            public Change MergeResult { get; set; }

            public override void Undo()
            {
                UndoCount++;
            }

            public override void Redo()
            {
                RedoCount++;
            }

            public override bool TryMergeWith(Change previous, out Change result)
            {
                MergedChanges.Add(previous);
                result = MergeResult;
                return result != null;
            }
        }
    }
}
