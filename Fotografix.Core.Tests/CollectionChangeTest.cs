using Fotografix.Collections;
using NUnit.Framework;
using System.Collections.Generic;

namespace Fotografix
{
    [TestFixture]
    public class CollectionChangeTest
    {
        [Test]
        public void AddItem()
        {
            List<string> list = new List<string> { "foo", "bar" };

            IChange change = new AddItemChange<string>(list, 0, "foo");

            change.Undo();
            Assert.That(list, Is.EqualTo(new List<string> { "bar" }), "Undo");

            change.Redo();
            Assert.That(list, Is.EqualTo(new List<string> { "foo", "bar" }), "Redo");
        }

        [Test]
        public void RemoveItem()
        {
            List<string> list = new List<string> { "bar" };

            IChange change = new RemoveItemChange<string>(list, 0, "foo");

            change.Undo();
            Assert.That(list, Is.EqualTo(new List<string> { "foo", "bar" }), "Undo");

            change.Redo();
            Assert.That(list, Is.EqualTo(new List<string> { "bar" }), "Redo");
        }

        [Test]
        public void ReplaceItem()
        {
            List<string> list = new List<string> { "bar" };

            IChange change = new ReplaceItemChange<string>(list, 0, "foo", "bar");

            change.Undo();
            Assert.That(list, Is.EqualTo(new List<string> { "foo" }), "Undo");

            change.Redo();
            Assert.That(list, Is.EqualTo(new List<string> { "bar" }), "Redo");
        }

        [Test]
        public void MoveItem()
        {
            List<string> list = new List<string> { "bar", "foo" };

            IChange change = new MoveItemChange<string>(list, 0, 1);

            change.Undo();
            Assert.That(list, Is.EqualTo(new List<string> { "foo", "bar" }), "Undo");

            change.Redo();
            Assert.That(list, Is.EqualTo(new List<string> { "bar", "foo" }), "Redo");
        }

        [Test]
        public void RemoveAndAddMergeIntoMove()
        {
            List<string> list = new List<string>();

            IChange remove = new RemoveItemChange<string>(list, 0, "foo");
            IMergeableChange add = new AddItemChange<string>(list, 1, "foo");

            bool succeeded = add.TryMergeWith(remove, out IChange result);

            Assert.IsTrue(succeeded);
            Assert.That(result, Is.EqualTo(new MoveItemChange<string>(list, 0, 1)));
        }

        [Test]
        public void RemoveAndAddDifferentItemsIsNotValidMove()
        {
            List<string> list = new List<string>();

            IChange remove = new RemoveItemChange<string>(list, 0, "foo");
            IMergeableChange add = new AddItemChange<string>(list, 1, "bar");
            
            bool succeeded = add.TryMergeWith(remove, out _);

            Assert.IsFalse(succeeded);
        }

        [Test]
        public void RemoveAndAddAcrossDifferentListsIsNotValidMove()
        {
            IChange remove = new RemoveItemChange<string>(new List<string>(), 0, "foo");
            IMergeableChange add = new AddItemChange<string>(new List<string>(), 1, "foo");
            
            bool succeeded = add.TryMergeWith(remove, out _);

            Assert.IsFalse(succeeded);
        }
    }
}
