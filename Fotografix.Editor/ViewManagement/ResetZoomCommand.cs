namespace Fotografix.Editor.ViewManagement
{
    public sealed class ResetZoomCommand : SynchronousDocumentCommand
    {
        public override void Execute(Document document, object parameter)
        {
            document.Viewport.ZoomToFit = false;
            document.Viewport.ZoomFactor = 1;
        }
    }
}
