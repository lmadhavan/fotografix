namespace Fotografix.Editor.ViewManagement
{
    public sealed class ZoomInCommand : SynchronousDocumentCommand
    {
        public override void Execute(Document document, object parameter)
        {
            document.Viewport.ZoomFactor *= 2;
        }
    }
}
