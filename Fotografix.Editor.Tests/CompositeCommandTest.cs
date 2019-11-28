using NUnit.Framework;
using System.Collections.Generic;

namespace Fotografix.Editor.Tests
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
        public void ExecutesCommandsInSpecifiedOrder()
        {
            compositeCommand.Execute();
            Assert.That(results, Is.EqualTo(new string[] { "Execute1", "Execute2" }));
        }

        [Test]
        public void UndoesCommandsInReverseOrder()
        {
            compositeCommand.Undo();
            Assert.That(results, Is.EqualTo(new string[] { "Undo2", "Undo1" }));
        }

        [Test]
        public void RedoesCommandsInSpecifiedOrder()
        {
            compositeCommand.Redo();
            Assert.That(results, Is.EqualTo(new string[] { "Redo1", "Redo2" }));
        }

        private sealed class FakeCommand : ICommand
        {
            private readonly List<string> results;
            private readonly int i;

            public FakeCommand(List<string> results, int i)
            {
                this.results = results;
                this.i = i;
            }

            public void Execute()
            {
                results.Add("Execute" + i);
            }

            public void Undo()
            {
                results.Add("Undo" + i);
            }

            public void Redo()
            {
                results.Add("Redo" + i);
            }
        }
    }
}
