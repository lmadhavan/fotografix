using Fotografix.Editor.Commands;
using NUnit.Framework;
using System.Collections.Generic;

namespace Fotografix.Editor.Tests.Commands
{
    [TestFixture]
    public class CompositeCommandTest
    {
        private List<string> results;
        private FakeCommand command1;
        private FakeCommand command2;
        private CompositeCommand compositeCommand;

        [SetUp]
        public void SetUp()
        {
            this.results = new List<string>();
            this.command1 = new FakeCommand(results, 1);
            this.command2 = new FakeCommand(results, 2);
            this.compositeCommand = new CompositeCommand(command1, command2);
        }

        [Test]
        public void PreparesChanges()
        {
            compositeCommand.PrepareChange();
            Assert.That(results, Is.EquivalentTo(new string[] { "Prepare1", "Prepare2" }));
        }

        [Test]
        public void AppliesChangesInSpecifiedOrder()
        {
            IChange change = compositeCommand.PrepareChange();
            results.Clear();
            change.Apply();

            Assert.That(results, Is.EqualTo(new string[] { "Apply1", "Apply2" }));
        }

        [Test]
        public void UndoesChangesInReverseOrder()
        {
            IChange change = compositeCommand.PrepareChange();
            results.Clear();
            change.Undo();

            Assert.That(results, Is.EqualTo(new string[] { "Undo2", "Undo1" }));
        }

        private sealed class FakeCommand : ICommand, IChange
        {
            private readonly List<string> results;
            private readonly int i;

            public FakeCommand(List<string> results, int i)
            {
                this.results = results;
                this.i = i;
            }

            public IChange PrepareChange()
            {
                results.Add("Prepare" + i);
                return this;
            }

            public void Apply()
            {
                results.Add("Apply" + i);
            }

            public void Undo()
            {
                results.Add("Undo" + i);
            }
        }
    }
}
