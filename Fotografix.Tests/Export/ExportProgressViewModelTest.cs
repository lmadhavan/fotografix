using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Windows.UI.Xaml;

namespace Fotografix.Export
{
    [TestClass]
    public class ExportProgressViewModelTest
    {
        private CancellationTokenSource cts;
        private ExportProgressViewModel vm;

        [TestInitialize]
        public void Initialize()
        {
            this.cts = new CancellationTokenSource();
            this.vm = new ExportProgressViewModel(cts);
        }

        [TestCleanup]
        public void Cleanup()
        {
            cts.Dispose();
        }

        [TestMethod]
        public void ExportNotStarted()
        {
            Assert.IsFalse(vm.IsOpen);
        }

        [TestMethod]
        public void ExportInProgress()
        {
            vm.Report(new ExportProgress { TotalItems = 2, ProcessedItems = 1 });

            Assert.IsTrue(vm.IsOpen);
            Assert.IsFalse(vm.CanClose);
            Assert.AreEqual(Visibility.Visible, vm.CancelButtonVisibility);
            Assert.AreEqual(InfoBarSeverity.Informational, vm.Severity);
            Assert.AreEqual("1 of 2 processed", vm.Status);
        }

        [TestMethod]
        public void ExportInProgressWithFailures()
        {
            vm.Report(new ExportProgress { TotalItems = 2, ProcessedItems = 1, FailedItems = 1 });

            Assert.IsTrue(vm.IsOpen);
            Assert.IsFalse(vm.CanClose);
            Assert.AreEqual(Visibility.Visible, vm.CancelButtonVisibility);
            Assert.AreEqual(InfoBarSeverity.Error, vm.Severity);
            Assert.AreEqual("1 of 2 processed (1 failed)", vm.Status);
        }

        [TestMethod]
        public void ExportComplete()
        {
            vm.Report(new ExportProgress { TotalItems = 2, ProcessedItems = 2 });

            Assert.IsTrue(vm.IsOpen);
            Assert.IsTrue(vm.CanClose);
            Assert.AreEqual(Visibility.Collapsed, vm.CancelButtonVisibility);
            Assert.AreEqual(InfoBarSeverity.Success, vm.Severity);
            Assert.AreEqual("2 of 2 processed", vm.Status);
        }

        [TestMethod]
        public void ExportCompleteWithFailures()
        {
            vm.Report(new ExportProgress { TotalItems = 2, ProcessedItems = 2, FailedItems = 1 });

            Assert.IsTrue(vm.IsOpen);
            Assert.IsTrue(vm.CanClose);
            Assert.AreEqual(Visibility.Collapsed, vm.CancelButtonVisibility);
            Assert.AreEqual(InfoBarSeverity.Error, vm.Severity);
            Assert.AreEqual("2 of 2 processed (1 failed)", vm.Status);
        }

        [TestMethod]
        public void ExportCancelled()
        {
            vm.Report(new ExportProgress { TotalItems = 2, ProcessedItems = 1 });
            vm.Cancel();

            Assert.IsTrue(vm.IsOpen);
            Assert.IsTrue(vm.CanClose);
            Assert.AreEqual(Visibility.Collapsed, vm.CancelButtonVisibility);
            Assert.AreEqual(InfoBarSeverity.Warning, vm.Severity);
            Assert.AreEqual("1 of 2 processed (cancelled)", vm.Status);
        }
    }
}
