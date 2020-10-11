using Fotografix.Uwp.FileManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class RecentFileListTest
    {
        private StorageItemMostRecentlyUsedList mruList;
        private RecentFileList recentFiles;
        private StorageFile file;

        [TestInitialize]
        public async Task Initialize()
        {
            this.mruList = StorageApplicationPermissions.MostRecentlyUsedList;
            mruList.Clear();

            this.recentFiles = new RecentFileList(mruList);
            this.file = await TestImages.GetFileAsync("flowers.jpg");
        }

        [TestMethod]
        public void AddsFileToMostRecentlyUsedList()
        {
            recentFiles.Add(file);

            Assert.AreEqual(1, mruList.Entries.Count);
            Assert.AreEqual(1, recentFiles.Count);
            Assert.AreEqual(file.DisplayName, recentFiles[0].DisplayName);
        }

        [TestMethod]
        public async Task GetsOriginalFileFromRecentFileEntry()
        {
            recentFiles.Add(file);

            StorageFile recentFile = await recentFiles.GetFileAsync(recentFiles[0]);
            Assert.IsTrue(recentFile.IsEqual(file));
        }

        [TestMethod]
        public void ObservesRemovalFromMostRecentlyUsedList()
        {
            recentFiles.Add(file);

            AssertCollectionChanged(recentFiles, () => mruList.Clear());
        }

        private void AssertCollectionChanged(INotifyCollectionChanged collection, Action action)
        {
            bool changed = false;
            collection.CollectionChanged += (s, e) => changed = true;
            action();
            Assert.IsTrue(changed);
        }
    }
}
