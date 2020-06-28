using Fotografix.Editor;
using Fotografix.Editor.Tools;
using System.Collections.Generic;
using System.Drawing;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Fotografix.UI
{
    public sealed class ToolAdapter
    {
        private static readonly Dictionary<ToolCursor, CoreCursor> CursorMap = new Dictionary<ToolCursor, CoreCursor>
        {
            [ToolCursor.Disabled] = new CoreCursor(CoreCursorType.UniversalNo, 0),
            [ToolCursor.Crosshair] = new CoreCursor(CoreCursorType.Cross, 0),
            [ToolCursor.OpenHand] = new CoreCursor(CoreCursorType.Custom, 101),
            [ToolCursor.ClosedHand] = new CoreCursor(CoreCursorType.Custom, 102)
        };

        private readonly CoreWindow window;
        private readonly UIElement canvas;
        private readonly UIElement viewport;

        private CoreCursor originalCursor;

        public ToolAdapter(UIElement canvas, UIElement viewport)
        {
            this.window = CoreWindow.GetForCurrentThread();
            this.canvas = canvas;
            this.viewport = viewport;
        }

        public IToolbox Toolbox { get; set; }
        private ITool ActiveTool => Toolbox?.ActiveTool;

        public void PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.originalCursor = window.PointerCursor;
        }

        public void PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            canvas.CapturePointer(e.Pointer);
            ActiveTool?.PointerPressed(PointerStateFrom(e));
            UpdateCursor();
        }

        public void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            ActiveTool?.PointerMoved(PointerStateFrom(e));
            UpdateCursor();
        }

        public void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ActiveTool?.PointerReleased(PointerStateFrom(e));
            UpdateCursor();
            canvas.ReleasePointerCapture(e.Pointer);
        }

        public void PointerExited(object sender, PointerRoutedEventArgs e)
        {
            window.PointerCursor = originalCursor;
        }

        private void UpdateCursor()
        {
            window.PointerCursor = CursorMap[ActiveTool?.Cursor ?? ToolCursor.Disabled];
        }

        private PointerState PointerStateFrom(PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(canvas);
            PointerPoint viewportPoint = e.GetCurrentPoint(viewport);

            return new PointerState(
                location: new Point((int)point.Position.X, (int)point.Position.Y),
                viewportLocation: new PointF((float)viewportPoint.Position.X, (float)viewportPoint.Position.Y)
            );
        }
    }
}
