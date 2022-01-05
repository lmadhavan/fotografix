using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Export
{
    [TestClass]
    public class ExportViewModelTest
    {
        private StorageFolder tempFolder;
        private ExportViewModel vm;

        [TestInitialize]
        public void Initialize()
        {
            this.tempFolder = ApplicationData.Current.TemporaryFolder;
            this.vm = new ExportViewModel(tempFolder);
        }

        [TestMethod]
        public void DefaultSettingsAreValid()
        {
            Assert.IsTrue(vm.IsValid, nameof(vm.IsValid));
        }

        [TestMethod]
        public void SubfolderNameCannotBeEmptyWhenPutInSubfolderIsChecked()
        {
            vm.PutInSubfolder = true;
            vm.SubfolderName = " ";

            Assert.IsFalse(vm.IsValid, nameof(vm.IsValid));
        }

        [TestMethod]
        public void SubfolderNameCannotContainInvalidFileNameCharacters()
        {
            vm.PutInSubfolder = true;
            vm.SubfolderName = "a\\b";

            Assert.IsFalse(vm.IsValid, nameof(vm.IsValid));
            Assert.IsTrue(vm.IsSubfolderNameInvalid, nameof(vm.IsSubfolderNameInvalid));
        }

        [TestMethod]
        public void NormalSubfolderNameIsValid()
        {
            vm.PutInSubfolder = true;
            vm.SubfolderName = "a";

            Assert.IsTrue(vm.IsValid, nameof(vm.IsValid));
        }

        [TestMethod]
        public async Task ResolvesDestinationFolderWithoutSubfolder()
        {
            vm.PutInSubfolder = false;

            var eo = await vm.CreateExportOptionsAsync();

            Assert.AreEqual(tempFolder.Path, eo.DestinationFolder.Path);
        }

        [TestMethod]
        public async Task ResolvesDestinationFolderWithSubfolder()
        {
            vm.PutInSubfolder = true;
            vm.SubfolderName = "a";

            var eo = await vm.CreateExportOptionsAsync();

            Assert.AreEqual(Path.Combine(tempFolder.Path, "a"), eo.DestinationFolder.Path);
        }

        [TestMethod]
        public async Task MaxDimensionIsUnsetWhenResizeOutputIsUnchecked()
        {
            vm.ResizeOutput = false;

            var eo = await vm.CreateExportOptionsAsync();

            Assert.IsNull(eo.MaxDimension);
        }

        [TestMethod]
        public async Task MaxDimensionIsSetWhenResizeOutputIsUnchecked()
        {
            vm.ResizeOutput = true;
            vm.ResizeDimension = 100;

            var eo = await vm.CreateExportOptionsAsync();

            Assert.AreEqual(100, eo.MaxDimension.Value);
        }

        [TestMethod]
        public async Task ConvertsQualityValueToFraction()
        {
            vm.Quality = 50;

            var eo = await vm.CreateExportOptionsAsync();

            Assert.AreEqual(0.5f, eo.Quality);
        }
    }
}
