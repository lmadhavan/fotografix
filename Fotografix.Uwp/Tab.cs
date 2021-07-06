using Fotografix.Uwp.FileManagement;
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
            this.IsEmpty = true;
        }

        public void Dispose()
        {
            if (frame.Content is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public bool IsEmpty { get; private set; }
        public Type ContentType => frame.Content.GetType();

        public void OpenStartPage(FileManager fileManager)
        {
            this.Header = "Start";
            frame.Navigate(typeof(StartPage), fileManager, new SuppressNavigationTransitionInfo());
        }

        public void OpenImageEditor(ICreateImageEditorCommand command)
        {
            frame.Navigate(typeof(ImageEditorPage), command, new SuppressNavigationTransitionInfo());

            ImageEditorPage page = (ImageEditorPage)frame.Content;
            this.Header = page.Title;
            page.TitleChanged += (s, e) => this.Header = page.Title;

            this.IsEmpty = false;
        }
    }
}
