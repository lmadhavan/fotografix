namespace Fotografix.Editor.ViewManagement
{
    public sealed class ZoomToFitCommand : SynchronousDocumentCommand
    {
        public override void Execute(Document document, object parameter)
        {
            document.Viewport.ZoomToFit = true;
        }
    }
}
