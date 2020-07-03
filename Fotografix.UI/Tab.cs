using Fotografix.UI.FileManagement;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;

namespace Fotografix.UI
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

        public void OpenStartPage(Workspace workspace)
        {
            this.Header = "Start";
            frame.Navigate(typeof(StartPage), workspace, new SuppressNavigationTransitionInfo());
        }

        public void OpenImageEditor(ICreateImageEditorCommand command)
        {
            this.Header = command.Title;
            frame.Navigate(typeof(ImageEditorPage), command, new SuppressNavigationTransitionInfo());
            this.IsEmpty = false;
        }
    }
}
