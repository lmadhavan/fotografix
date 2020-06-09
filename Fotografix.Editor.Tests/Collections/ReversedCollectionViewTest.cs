using NUnit.Framework;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Fotografix.Editor.Collections
{
    [TestFixture]
    public class ReversedCollectionViewTest
    {
        private ObservableCollection<string> collection;
        private ReversedCollectionView<string> view;
        private NotifyCollectionChangedEventArgs e;

        [SetUp]
        public void SetUp()
        {
            this.collection = new ObservableCollection<string>();
            collection.Add("foo");
            collection.Add("bar");

            this.view = new ReversedCollectionView<string>(collection);
            view.CollectionChanged += (s, e) => this.e = e;
        }

        [Test]
        public void AddToUnderlyingCollection()
        {
            collection.Add("baz"); // collection -> foo, bar, baz <- view

            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.AreEqual(0, e.NewStartingIndex);
            Assert.AreEqual("baz", view[0]);
        }

        [Test]
        public void RemoveFromUnderlyingCollection()
        {
            collection.Remove("foo"); // collection -> bar <- view

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
            Assert.AreEqual(1, e.OldStartingIndex);
        }

        [Test]
        public void ReplaceInUnderlyingCollection()
        {
            collection[1] = "baz"; // collection -> foo, baz <- view

            Assert.AreEqual(NotifyCollectionChangedAction.Replace, e.Action);
            Assert.AreEqual(0, e.OldStartingIndex);
            Assert.AreEqual(0, e.NewStartingIndex);
            Assert.AreEqual("baz", view[0]);
        }

        [Test]
        public void MoveInUnderlyingCollection()
        {
            collection.Move(0, 1); // collection -> baz, foo <- view

            Assert.AreEqual(NotifyCollectionChangedAction.Move, e.Action);
            Assert.AreEqual(1, e.OldStartingIndex);
            Assert.AreEqual(0, e.NewStartingIndex);
        }

        [Test]
        public void AddToView()
        {
            view.Add("baz"); // view -> bar, foo, baz <- collection

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual("baz", collection[0]);
        }

        [Test]
        public void InsertInView()
        {
            view.Insert(1, "baz"); // view -> bar, baz, foo <- collection

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual("baz", collection[1]);
        }

        [Test]
        public void RemoveFromView()
        {
            view.RemoveAt(0); // view -> foo <- collection

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("foo", collection[0]);
        }

        [Test]
        public void ReplaceInView()
        {
            view[0] = "baz"; // view -> baz, foo <- collection

            Assert.AreEqual("baz", collection[1]);
        }
    }
}
