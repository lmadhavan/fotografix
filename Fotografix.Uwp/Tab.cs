using Fotografix.Editor;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using BindableAttribute = Windows.UI.Xaml.Data.BindableAttribute;

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

        public void OpenImageEditor(ImageEditor editor)
        {
            editor.PropertyChanged += Editor_PropertyChanged;

            this.Header = editor.Title;
            this.Document = editor.Document;

            frame.Navigate(typeof(ImageEditorPage), editor, new SuppressNavigationTransitionInfo());
        }

        private void Editor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageEditor.Title))
            {
                this.Header = ((ImageEditor)sender).Title;
            }
        }
    }
}
