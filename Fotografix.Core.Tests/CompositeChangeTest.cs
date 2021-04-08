using NUnit.Framework;
using System.Collections.Generic;

namespace Fotografix
{
    [TestFixture]
    public class CompositeChangeTest
    {
        private List<string> results;
        private FakeChange change1;
        private FakeChange change2;
        private CompositeChange compositeChange;

        [SetUp]
        public void SetUp()
        {
            this.results = new List<string>();
            this.change1 = new FakeChange(results, 1);
            this.change2 = new FakeChange(results, 2);
            this.compositeChange = new CompositeChange(change1, change2);
        }

        [Test]
        public void UndoesChangesInReverseOrder()
        {
            compositeChange.Undo();

            Assert.That(results, Is.EqualTo(new string[] { "Undo2", "Undo1" }));
        }

        [Test]
        public void RedoesChangesInSpecifiedOrder()
        {
            compositeChange.Redo();

            Assert.That(results, Is.EqualTo(new string[] { "Redo1", "Redo2" }));
        }

        private sealed class FakeChange : IChange
        {
            private readonly List<string> results;
            private readonly int marker;

            public FakeChange(List<string> results, int marker)
            {
                this.results = results;
                this.marker = marker;
            }

            public void Undo()
            {
                results.Add("Undo" + marker);
            }

            public void Redo()
            {
                results.Add("Redo" + marker);
            }
        }
    }
}
