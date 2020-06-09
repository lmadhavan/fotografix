using NUnit.Framework;
using System.Collections.Generic;

namespace Fotografix.Editor
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
        public void ExecutesAllCommands()
        {
            compositeCommand.Execute();
            Assert.That(results, Is.EqualTo(new string[] { "Execute1", "Execute2" }));
        }

        [Test]
        public void UndoesAllCommandsInReverseOrder()
        {
            compositeCommand.Undo();

            Assert.That(results, Is.EqualTo(new string[] { "Undo2", "Undo1" }));
        }

        private sealed class FakeCommand : Command
        {
            private readonly List<string> results;
            private readonly int i;

            public FakeCommand(List<string> results, int i)
            {
                this.results = results;
                this.i = i;
            }

            public override void Execute()
            {
                results.Add("Execute" + i);
            }

            public override void Undo()
            {
                results.Add("Undo" + i);
            }
        }
    }
}
