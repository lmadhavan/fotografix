using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Fotografix.Editor.Collections
{
    [TestFixture]
    public class ReorderAwareCollectionViewTest
    {
        private List<string> list;
        private ReorderAwareCollectionView<string> view;
        private ItemReorderedEventArgs eventArgs;

        [SetUp]
        public void SetUp()
        {
            this.list = new List<string> { "foo", "bar" };
            this.view = new ReorderAwareCollectionView<string>(list);
            view.ItemReordered += (s, e) => this.eventArgs = e;
        }

        [Test]
        public void RemoveAndInsertSameElementTriggersReorderEvent()
        {
            // when I remove an element and insert it at another index
            string s = view[0];
            view.RemoveAt(0);
            view.Insert(1, s);

            // then a reorder event is raised
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(0, eventArgs.OldIndex);
            Assert.AreEqual(1, eventArgs.NewIndex);
        }

        [Test]
        public void RemoveElementAndInsertDifferentElementThrowsException()
        {
            // when I remove an element
            view.RemoveAt(0);

            // then trying to insert a different element throws an exception
            Assert.Throws<InvalidOperationException>(() => view.Insert(1, "baz"));
        }

        [Test]
        public void TwoSuccessiveRemovesThrowsException()
        {
            // when I remove an element
            view.RemoveAt(0);

            // then trying to immediately remove another element throws an exception
            Assert.Throws<InvalidOperationException>(() => view.RemoveAt(0));
        }

        [Test]
        public void TwoSuccessiveInsertsThrowsException()
        {
            // when I remove an element and insert it at another index
            string s = view[0];
            view.RemoveAt(0);
            view.Insert(1, s);

            // then trying to immediately insert another element throws an exception
            Assert.Throws<InvalidOperationException>(() => view.Insert(1, s));
        }
    }
}
