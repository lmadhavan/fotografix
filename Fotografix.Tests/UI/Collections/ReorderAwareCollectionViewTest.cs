using Fotografix.UI.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Fotografix.Tests.UI.Collections
{
    [TestClass]
    public class ReorderAwareCollectionViewTest
    {
        private List<string> list;
        private ReorderAwareCollectionView<string> view;
        private ItemReorderedEventArgs eventArgs;

        [TestInitialize]
        public void Initialize()
        {
            this.list = new List<string> { "foo", "bar" };
            this.view = new ReorderAwareCollectionView<string>(list);
            view.ItemReordered += (s, e) => this.eventArgs = e;
        }

        [TestMethod]
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

        [TestMethod]
        public void RemoveElementAndInsertDifferentElementThrowsException()
        {
            // when I remove an element
            view.RemoveAt(0);

            // then trying to insert a different element throws an exception
            Assert.ThrowsException<InvalidOperationException>(() => view.Insert(1, "baz"));
        }

        [TestMethod]
        public void TwoSuccessiveRemovesThrowsException()
        {
            // when I remove an element
            view.RemoveAt(0);

            // then trying to immediately remove another element throws an exception
            Assert.ThrowsException<InvalidOperationException>(() => view.RemoveAt(0));
        }

        [TestMethod]
        public void TwoSuccessiveInsertsThrowsException()
        {
            // when I remove an element and insert it at another index
            string s = view[0];
            view.RemoveAt(0);
            view.Insert(1, s);

            // then trying to immediately insert another element throws an exception
            Assert.ThrowsException<InvalidOperationException>(() => view.Insert(1, s));
        }
    }
}
