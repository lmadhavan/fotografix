﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Fotografix
{
    [TestClass]
    public class ApplicationViewModelTest
    {
        private ApplicationViewModel app;

        [TestInitialize]
        public async Task Initialize()
        {
            var testData = await Package.Current.InstalledLocation.GetFolderAsync("TestData");
            var photosFolder = await testData.GetFolderAsync("Photos");
            var sidecarStrategy = new FixedSidecarStrategy(ApplicationData.Current.TemporaryFolder);

            this.app = new ApplicationViewModel(sidecarStrategy) { CanvasResourceCreator = new StubCanvasResourceCreator() };
            app.Folder = photosFolder;
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await app.DisposeAsync();
        }

        [TestMethod]
        public async Task NoPhotoSelected()
        {
            var photos = await app.Photos.Task;

            SelectPhoto(photos[0]);
            Assert.IsNotNull(await app.Editor.Task);

            ClearSelection();
            Assert.IsNull(await app.Editor.Task);

            SelectPhoto(photos[1]);
            Assert.IsNotNull(await app.Editor.Task);
        }

        [TestMethod]
        public async Task RecoversFromEditorFailure()
        {
            var photos = await app.Photos.Task;

            SelectPhoto(photos[2]); // ZZZ_BadImage.jpg
            SelectPhoto(photos[0]);

            Assert.IsNotNull(await app.Editor.Task);
        }

        [TestMethod]
        public async Task ClosesActiveEditorWhenMultiplePhotosSelected()
        {
            var photos = await app.Photos.Task;

            SelectPhoto(photos[0]);
            SelectPhotos(photos[0], photos[1]);

            Assert.IsNull(await app.Editor.Task);
        }

        private void SelectPhoto(PhotoViewModel photo)
        {
            SelectPhotos(photo);
        }

        private void ClearSelection()
        {
            SelectPhotos();
        }

        private void SelectPhotos(params PhotoViewModel[] photos)
        {
            app.SelectedPhotos = photos;
        }
    }
}
