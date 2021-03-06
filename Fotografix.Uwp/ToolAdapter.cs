﻿using Fotografix.Editor;
using Fotografix.Editor.Tools;
using System.Collections.Generic;
using System.Drawing;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Fotografix.Uwp
{
    public sealed class ToolAdapter
    {
        public static readonly Dictionary<ToolCursor, CoreCursor> CursorMap = new Dictionary<ToolCursor, CoreCursor>
        {
            [ToolCursor.Disabled] = new CoreCursor(CoreCursorType.UniversalNo, 0),
            [ToolCursor.Crosshair] = new CoreCursor(CoreCursorType.Cross, 0),
            [ToolCursor.OpenHand] = new CoreCursor(CoreCursorType.Custom, 101),
            [ToolCursor.ClosedHand] = new CoreCursor(CoreCursorType.Custom, 102),
            [ToolCursor.Move] = new CoreCursor(CoreCursorType.SizeAll, 0),
            [ToolCursor.SizeNortheastSouthwest] = new CoreCursor(CoreCursorType.SizeNortheastSouthwest, 0),
            [ToolCursor.SizeNorthSouth] = new CoreCursor(CoreCursorType.SizeNorthSouth, 0),
            [ToolCursor.SizeNorthwestSoutheast] = new CoreCursor(CoreCursorType.SizeNorthwestSoutheast, 0),
            [ToolCursor.SizeWestEast] = new CoreCursor(CoreCursorType.SizeWestEast, 0),
        };

        private readonly CoreWindow window;
        private readonly UIElement canvas;
        private readonly Viewport viewport;

        private bool pointerInsideViewport;
        private bool pointerCaptured;
        private CoreCursor originalCursor;

        public ToolAdapter(UIElement canvas, Viewport viewport)
        {
            this.window = CoreWindow.GetForCurrentThread();
            this.canvas = canvas;
            this.viewport = viewport;
        }

        public IToolbox Toolbox { get; set; }
        private ITool ActiveTool => Toolbox?.ActiveTool;

        public void PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.pointerInsideViewport = true;

            if (!pointerCaptured)
            {
                this.originalCursor = window.PointerCursor;
            }
        }

        public void PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (canvas.CapturePointer(e.Pointer))
            {
                this.pointerCaptured = true;
            }

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

            canvas.ReleasePointerCapture(e.Pointer);
            this.pointerCaptured = false;

            if (pointerInsideViewport)
            {
                UpdateCursor();
            }
            else
            {
                RestoreCursor();
            }
        }

        public void PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!pointerCaptured)
            {
                RestoreCursor();
            }

            this.pointerInsideViewport = false;
        }

        private void RestoreCursor()
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
            Point pt = new Point((int)point.Position.X, (int)point.Position.Y);
            return new PointerState(viewport.TransformViewportToImage(pt));
        }
    }
}
