using NUnit.Framework;

namespace Fotografix
{
    [TestFixture]
    public class PropertyChangeTest
    {
        [Test]
        public void ChangesObjectProperty()
        {
            TestObject obj = new TestObject();

            Change change = new PropertyChange(obj, nameof(TestObject.Name), "foo", "bar");

            change.Undo();
            Assert.That(obj.Name, Is.EqualTo("foo"), "Undo");

            change.Redo();
            Assert.That(obj.Name, Is.EqualTo("bar"), "Redo");
        }

        [Test]
        public void MergesConsecutiveChangesToSameProperty()
        {
            TestObject obj = new TestObject();

            Change change1 = new PropertyChange(obj, nameof(TestObject.Name), "foo", "bar");
            Change change2 = new PropertyChange(obj, nameof(TestObject.Name), "bar", "baz");

            bool succeeded = change2.TryMergeWith(change1, out Change result);

            Assert.IsTrue(succeeded);
            Assert.That(result, Is.EqualTo(new PropertyChange(obj, nameof(TestObject.Name), "foo", "baz")));
        }

        [Test]
        public void DoesNotMergeChangesToDifferentObjects()
        {
            Change change1 = new PropertyChange(new TestObject(), nameof(TestObject.Name), "foo", "bar");
            Change change2 = new PropertyChange(new TestObject(), nameof(TestObject.Name), "bar", "baz");
            
            bool succeeded = change2.TryMergeWith(change1, out _);

            Assert.IsFalse(succeeded);
        }

        [Test]
        public void DoesNotMergeChangesToDifferentProperties()
        {
            TestObject obj = new TestObject();

            Change change1 = new PropertyChange(obj, nameof(TestObject.Name), "foo", "bar");
            Change change2 = new PropertyChange(obj, nameof(TestObject.Count), 1, 2);

            bool succeeded = change2.TryMergeWith(change1, out _);

            Assert.IsFalse(succeeded);
        }

        private class TestObject
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }
}
