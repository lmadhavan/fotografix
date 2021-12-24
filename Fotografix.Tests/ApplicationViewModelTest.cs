using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            app.OpenFolder(photosFolder);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await app.DisposeAsync();
        }

        /// <summary>
        /// A missing null check in the view model breaks the editor load task for the rest of the session.
        /// </summary>
        [TestMethod]
        public async Task HandlesNullSelectedPhoto()
        {
            var photos = await app.Photos.Task;
            
            app.SelectedPhoto = photos[0];
            Assert.IsNotNull(await app.Editor.Task);

            app.SelectedPhoto = null;
            Assert.IsNull(await app.Editor.Task);

            app.SelectedPhoto = photos[1];
            Assert.IsNotNull(await app.Editor.Task);
        }

        /// <summary>
        /// Missing error handling in the view model breaks the editor load task for the rest of the session.
        /// </summary>
        [TestMethod]
        public async Task HandlesFailedEditorLoadTask()
        {
            var photos = await app.Photos.Task;

            app.SelectedPhoto = photos[2]; // ZZZ_BadImage.jpg
            app.SelectedPhoto = photos[0];

            Assert.IsNotNull(await app.Editor.Task);
        }

        [TestMethod]
        public async Task InitializesDefaultExportFolderInEditor()
        {
            var photos = await app.Photos.Task;

            app.SelectedPhoto = photos[0];

            var editor = await app.Editor.Task;
            Assert.AreEqual(app.FolderName, editor.DefaultExportFolder.Name);
        }
    }
}
