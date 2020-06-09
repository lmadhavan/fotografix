using NUnit.Framework;

namespace Fotografix.Editor
{
    [TestFixture]
    public class ChangePropertyCommandTest
    {
        [Test]
        public void ChangesObjectProperty()
        {
            TestObject obj = new TestObject() { Name = "foo" };

            Command command = new ChangePropertyCommand(obj, nameof(TestObject.Name), "bar");

            command.Execute();
            Assert.That(obj.Name, Is.EqualTo("bar"));

            command.Undo();
            Assert.That(obj.Name, Is.EqualTo("foo"));
        }

        [Test]
        public void MergesIntoExistingCommand()
        {
            TestObject obj = new TestObject() { Name = "foo" };

            Command command1 = new ChangePropertyCommand(obj, nameof(TestObject.Name), "bar");
            Command command2 = new ChangePropertyCommand(obj, nameof(TestObject.Name), "baz");

            Assert.IsTrue(command2.TryMergeInto(command1));

            command1.Execute();
            Assert.That(obj.Name, Is.EqualTo("baz"));

            command1.Undo();
            Assert.That(obj.Name, Is.EqualTo("foo"));
        }

        [Test]
        public void DoesNotMergeAcrossDifferentTargetObjects()
        {
            TestObject obj1 = new TestObject() { Name = "one" };
            TestObject obj2 = new TestObject() { Name = "two" };

            Command command1 = new ChangePropertyCommand(obj1, nameof(TestObject.Name), "bar");
            Command command2 = new ChangePropertyCommand(obj2, nameof(TestObject.Name), "baz");

            Assert.IsFalse(command2.TryMergeInto(command1));
        }

        [Test]
        public void DoesNotMergeAcrossDifferentProperties()
        {
            TestObject obj = new TestObject() { Name = "foo", Count = 1 };

            Command command1 = new ChangePropertyCommand(obj, nameof(TestObject.Name), "baz");
            Command command2 = new ChangePropertyCommand(obj, nameof(TestObject.Count), 3);

            Assert.IsFalse(command2.TryMergeInto(command1));
        }

        private class TestObject
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }
}
