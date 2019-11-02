using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class ReversedCollectionViewTest
    {
        private ObservableCollection<string> collection;
        private ReversedCollectionView<string> view;
        private NotifyCollectionChangedEventArgs e;

        [TestInitialize]
        public void Initialize()
        {
            this.collection = new ObservableCollection<string>();
            collection.Add("foo");
            collection.Add("bar");

            this.view = new ReversedCollectionView<string>(collection);
            view.CollectionChanged += (s, e) => this.e = e;
        }

        [TestMethod]
        public void AddToUnderlyingCollection()
        {
            collection.Add("baz"); // collection -> foo, bar, baz <- view

            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.AreEqual(0, e.NewStartingIndex);
            Assert.AreEqual("baz", view[0]);
        }

        [TestMethod]
        public void RemoveFromUnderlyingCollection()
        {
            collection.Remove("foo"); // collection -> bar <- view

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
            Assert.AreEqual(1, e.OldStartingIndex);
        }

        [TestMethod]
        public void ReplaceInUnderlyingCollection()
        {
            collection[1] = "baz"; // collection -> foo, baz <- view

            Assert.AreEqual(NotifyCollectionChangedAction.Replace, e.Action);
            Assert.AreEqual(0, e.OldStartingIndex);
            Assert.AreEqual(0, e.NewStartingIndex);
            Assert.AreEqual("baz", view[0]);
        }

        [TestMethod]
        public void AddToView()
        {
            view.Add("baz"); // view -> bar, foo, baz <- collection

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual("baz", collection[0]);
        }

        [TestMethod]
        public void InsertInView()
        {
            view.Insert(1, "baz"); // view -> bar, baz, foo <- collection

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual("baz", collection[1]);
        }

        [TestMethod]
        public void RemoveFromView()
        {
            view.RemoveAt(0); // view -> foo <- collection

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("foo", collection[0]);
        }

        [TestMethod]
        public void ReplaceInView()
        {
            view[0] = "baz"; // view -> baz, foo <- collection

            Assert.AreEqual("baz", collection[1]);
        }
    }
}
