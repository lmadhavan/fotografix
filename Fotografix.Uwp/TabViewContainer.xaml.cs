using Fotografix.Editor;
using Fotografix.Uwp.FileManagement;
using Microsoft.UI.Xaml.Controls;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Fotografix.Uwp
{
    public sealed partial class TabViewContainer : UserControl, ICustomTitleBarProvider
    {
        private readonly ImageEditorFactory imageEditorFactory;
        private readonly Workspace workspace;
        private readonly RecentFileList recentFiles;
        private readonly StartPageViewModel startPageViewModel;

        public TabViewContainer() : this(new Workspace())
        {
        }

        public TabViewContainer(Workspace workspace)
        {
            this.imageEditorFactory = new ImageEditorFactory(workspace, ClipboardAdapter.GetForCurrentThread());

            this.workspace = workspace;
            workspace.DocumentAdded += Workspace_DocumentAdded;
            workspace.PropertyChanged += Workspace_PropertyChanged;

            this.recentFiles = new RecentFileList();

            this.startPageViewModel = new StartPageViewModel
            {
                NewCommand =  imageEditorFactory.NewCommand,
                OpenCommand = imageEditorFactory.OpenCommand,
                RecentFiles = recentFiles,
                FilePickerOverride = imageEditorFactory.FilePickerOverride
            };

            this.InitializeComponent();
            this.Tabs = new TabCollection(tabView);
        }

        public IReadOnlyList<Tab> Tabs { get; }

        public Tab ActiveTab
        {
            get => (Tab)tabView.SelectedItem;

            set
            {
                if (tabView.SelectedItem != value)
                {
                    tabView.SelectedItem = value;

                    // TabView does not raise SelectionChanged when SelectedItem is changed programatically
                    OnSelectionChanged();
                }
            }
        }

        public void OpenStartPage()
        {
            CreateEmptyTab().OpenStartPage(startPageViewModel);
        }

        public void OpenDocument(Document document)
        {
            GetOrCreateEmptyTab().OpenImageEditor(imageEditorFactory, document);
        }

        private Tab GetOrCreateEmptyTab()
        {
            Tab tab = ActiveTab;

            if (tab != null && tab.IsEmpty)
            {
                return tab;
            }

            return CreateEmptyTab();
        }

        private Tab CreateEmptyTab()
        {
            Tab tab = new Tab();
            tabView.TabItems.Add(tab);
            ActiveTab = tab;

            return tab;
        }

        private void OnSelectionChanged()
        {
            workspace.ActiveDocument = ActiveTab?.Document;
        }

        private void OnActiveDocumentChanged()
        {
            ActiveTab = Tabs.FirstOrDefault(t => t.Document == workspace.ActiveDocument);
        }

        private void Workspace_DocumentAdded(object sender, DocumentEventArgs e)
        {
            OpenDocument(e.Document);
        }

        private void Workspace_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Workspace.ActiveDocument))
            {
                OnActiveDocumentChanged();
            }
        }

        #region Tab collection wrapper
        private sealed class TabCollection : IReadOnlyList<Tab>
        {
            private readonly TabView tabView;

            public TabCollection(TabView tabView)
            {
                this.tabView = tabView;
            }

            public int Count => tabView.TabItems.Count;

            public Tab this[int index] => (Tab)tabView.TabItems[index];

            public IEnumerator<Tab> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        #endregion

        #region TabView event handlers

        private void TabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectionChanged();
        }

        private void TabView_NewTabRequested(TabView sender, object args)
        {
            OpenStartPage();
        }

        private void TabView_CloseTabRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            Tab tab = (Tab)args.Tab;
            tabView.TabItems.Remove(tab);

            if (tab.Document != null)
            {
                workspace.RemoveDocument(tab.Document);
            }

            tab.Dispose();
        }

        #endregion

        #region Keyboard accelerator event handlers

        private async void NewImage_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            await imageEditorFactory.NewCommand.ExecuteAsync();
        }

        private async void OpenFile_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            await imageEditorFactory.OpenCommand.ExecuteAsync();
        }

        #endregion

        #region Title bar customization

        UIElement ICustomTitleBarProvider.CustomTitleBar => tabStripFooter;

        void ICustomTitleBarProvider.UpdateLayout(ITitleBarLayoutMetrics metrics)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                tabStripHeader.MinWidth = metrics.SystemOverlayLeftInset;
                tabStripFooter.MinWidth = metrics.SystemOverlayRightInset;
            }
            else
            {
                tabStripHeader.MinWidth = metrics.SystemOverlayRightInset;
                tabStripFooter.MinWidth = metrics.SystemOverlayLeftInset;
            }

            tabStripHeader.Height = tabStripFooter.Height = metrics.Height;
        }

        #endregion
    }
}
