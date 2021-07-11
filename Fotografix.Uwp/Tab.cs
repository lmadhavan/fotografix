using Fotografix.Editor;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;

namespace Fotografix.Uwp
{
    [Bindable]
    public class Tab : TabViewItem
    {
        private readonly Frame frame;

        public Tab()
        {
            this.frame = new Frame();
            this.Header = " "; // TabView control blows up if this doesn't have at least one character
            this.Content = frame;
        }

        public void Dispose()
        {
            if (frame.Content is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public Document Document { get; private set; }

        public bool IsEmpty => Document == null;
        public Type ContentType => frame.Content.GetType();

        public void OpenStartPage(IStartPageViewModel viewModel)
        {
            this.Header = "Start";
            frame.Navigate(typeof(StartPage), viewModel, new SuppressNavigationTransitionInfo());
        }

        public void OpenImageEditor(WorkspaceViewModel imageEditorFactory, Document document)
        {
            this.Document = document;

            CreateImageEditorFunc createFunc = viewport => imageEditorFactory.CreateEditor(viewport, document);
            frame.Navigate(typeof(ImageEditorPage), createFunc, new SuppressNavigationTransitionInfo());

            ImageEditorPage page = (ImageEditorPage)frame.Content;
            this.Header = page.Title;
            page.TitleChanged += (s, e) => this.Header = page.Title;
        }
    }
}
