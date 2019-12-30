using Fotografix.Editor.Commands;
using NUnit.Framework;

namespace Fotografix.Editor.Tests.Commands
{
    [TestFixture]
    public class ChangePropertyCommandTest
    {
        [Test]
        public void ChangesObjectProperty()
        {
            TestObject obj = new TestObject() { Name = "foo" };

            IChange change = new ChangePropertyCommand(obj, nameof(TestObject.Name), "bar").PrepareChange();

            change.Apply();
            Assert.That(obj.Name, Is.EqualTo("bar"));

            change.Undo();
            Assert.That(obj.Name, Is.EqualTo("foo"));
        }

        [Test]
        public void ProducesNoChangeIfValueIsUnchanged()
        {
            TestObject obj = new TestObject() { Name = "foo" };

            IChange change = new ChangePropertyCommand(obj, nameof(TestObject.Name), obj.Name).PrepareChange();

            Assert.IsNull(change);
        }

        [Test]
        public void MergesIntoExistingChange()
        {
            TestObject obj = new TestObject() { Name = "foo" };

            IChange change1 = new ChangePropertyCommand(obj, nameof(TestObject.Name), "bar").PrepareChange();
            IChange change2 = new ChangePropertyCommand(obj, nameof(TestObject.Name), "baz").PrepareChange();

            Assert.IsTrue(((IMergeableChange)change2).TryMergeInto(change1));

            change1.Apply();
            Assert.That(obj.Name, Is.EqualTo("baz"));

            change1.Undo();
            Assert.That(obj.Name, Is.EqualTo("foo"));
        }

        [Test]
        public void DoesNotMergeAcrossDifferentTargetObjects()
        {
            TestObject obj1 = new TestObject() { Name = "one" };
            TestObject obj2 = new TestObject() { Name = "two" };

            IChange change1 = new ChangePropertyCommand(obj1, nameof(TestObject.Name), "bar").PrepareChange();
            IChange change2 = new ChangePropertyCommand(obj2, nameof(TestObject.Name), "baz").PrepareChange();

            Assert.IsFalse(((IMergeableChange)change2).TryMergeInto(change1));
        }

        [Test]
        public void DoesNotMergeAcrossDifferentProperties()
        {
            TestObject obj = new TestObject() { Name = "foo", Count = 1 };

            IChange change1 = new ChangePropertyCommand(obj, nameof(TestObject.Name), "baz").PrepareChange();
            IChange change2 = new ChangePropertyCommand(obj, nameof(TestObject.Count), 3).PrepareChange();

            Assert.IsFalse(((IMergeableChange)change2).TryMergeInto(change1));
        }

        private class TestObject
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }
}
