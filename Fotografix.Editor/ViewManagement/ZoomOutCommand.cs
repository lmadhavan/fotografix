namespace Fotografix.Editor.ViewManagement
{
    public sealed class ZoomOutCommand : SynchronousDocumentCommand
    {
        public override void Execute(Document document, object parameter)
        {
            document.Viewport.ZoomFactor /= 2;
        }
    }
}
