using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Fotografix
{
    [TestClass]
    public class RegressionTest
    {
        private ApplicationViewModel app;

        [TestInitialize]
        public async Task Initialize()
        {
            var testData = await Package.Current.InstalledLocation.GetFolderAsync("TestData");
            var photosFolder = await testData.GetFolderAsync("Photos");
            var sidecarStrategy = new FixedSidecarStrategy(ApplicationData.Current.TemporaryFolder);
            
            this.app = new ApplicationViewModel(sidecarStrategy);
            app.OpenFolder(photosFolder);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await app.DisposeAsync();
        }

        /// <summary>
        /// When switching folders, XAML sets SelectedPhoto to null when the Photos list is reset.
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
        /// Ensure that changes to EditorViewModel properties (such as ShowOriginal) and underlying
        /// PhotoAdjustment properties are propagated to ApplicationViewModel.
        /// </summary>
        [TestMethod]
        public async Task ForwardsEditorInvalidatedEvents()
        {
            var photos = await app.Photos.Task;
            app.SelectedPhoto = photos[0];
            var editor = await app.Editor.Task;

            int invalidatedCount = 0;
            app.EditorInvalidated += (s, e) => invalidatedCount++;

            editor.Adjustment.Exposure = 0.5f;
            editor.ShowOriginal = true;

            Assert.AreEqual(2, invalidatedCount);
        }
    }
}
