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

            this.view = new ReversedCollectionView<string>(new ReadOnlyObservableCollection<string>(collection));
            view.CollectionChanged += (s, e) => this.e = e;
        }

        [TestMethod]
        public void AddToUnderlyingCollection()
        {
            collection.Add("baz");

            Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
            Assert.AreEqual(0, e.NewStartingIndex);
            Assert.AreEqual("baz", view[0]);
        }

        [TestMethod]
        public void RemoveFromUnderlyingCollection()
        {
            collection.Remove("foo");

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
            Assert.AreEqual(1, e.OldStartingIndex);
        }

        [TestMethod]
        public void ReplaceInUnderlyingCollection()
        {
            collection[1] = "baz";

            Assert.AreEqual(NotifyCollectionChangedAction.Replace, e.Action);
            Assert.AreEqual(0, e.OldStartingIndex);
            Assert.AreEqual(0, e.NewStartingIndex);
            Assert.AreEqual("baz", view[0]);
        }
    }
}
