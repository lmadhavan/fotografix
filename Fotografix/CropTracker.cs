using Fotografix.Input;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Core;

namespace Fotografix
{
    public sealed class CropTracker : IPointerInputHandler
    {
        private static readonly Dictionary<Handle, CoreCursor> CursorDictionary = new Dictionary<Handle, CoreCursor>
        {
            [Handle.Disabled] = new CoreCursor(CoreCursorType.UniversalNo, 0),
            [Handle.TopLeft] = new CoreCursor(CoreCursorType.SizeNorthwestSoutheast, 0),
            [Handle.Top] = new CoreCursor(CoreCursorType.SizeNorthSouth, 0),
            [Handle.TopRight] = new CoreCursor(CoreCursorType.SizeNortheastSouthwest, 0),
            [Handle.Left] = new CoreCursor(CoreCursorType.SizeWestEast, 0),
            [Handle.Right] = new CoreCursor(CoreCursorType.SizeWestEast, 0),
            [Handle.BottomLeft] = new CoreCursor(CoreCursorType.SizeNortheastSouthwest, 0),
            [Handle.Bottom] = new CoreCursor(CoreCursorType.SizeNorthSouth, 0),
            [Handle.BottomRight] = new CoreCursor(CoreCursorType.SizeNorthwestSoutheast, 0),
            [Handle.Move] = new CoreCursor(CoreCursorType.SizeAll, 0),
        };

        private Rect rect;
        private bool tracking;
        private Handle handle;
        private Point startPoint;
        private Rect startRect;

        public CoreCursor Cursor => CursorDictionary[handle];

        public Rect Rect
        {
            get => rect;
            set => rect = value;
        }

        public Size MinSize { get; set; } = new Size(1, 1);
        public Rect MaxBounds { get; set; } = new Rect(0, 0, double.MaxValue, double.MaxValue);

        public event EventHandler RectChanged;

        public double HandleTolerance { get; set; }

        public bool PointerPressed(Point pt)
        {
            if (!tracking)
            {
                this.handle = HandleAt(pt);
                this.tracking = true;
                this.startPoint = pt;
                this.startRect = rect;
                return true;
            }

            return false;
        }

        public bool PointerMoved(Point pt)
        {
            if (tracking)
            {
                Track(pt);
            }
            else
            {
                this.handle = HandleAt(pt);
            }

            return true;
        }

        public bool PointerReleased(Point pt)
        {
            this.tracking = false;
            return true;
        }

        public static Rect RectFromLTRB(double l, double t, double r, double b, Size minSize = default)
        {
            return new Rect(l, t, Math.Max(r - l, minSize.Width), Math.Max(b - t, minSize.Height));
        }

        private void Track(Point pt)
        {
            double dx = pt.X - startPoint.X;
            double dy = pt.Y - startPoint.Y;

            switch (handle)
            {
                case Handle.TopLeft:
                    ResizeRect(dx, dy, 0, 0);
                    break;

                case Handle.Top:
                    ResizeRect(0, dy, 0, 0);
                    break;

                case Handle.TopRight:
                    ResizeRect(0, dy, dx, 0);
                    break;

                case Handle.Left:
                    ResizeRect(dx, 0, 0, 0);
                    break;

                case Handle.Right:
                    ResizeRect(0, 0, dx, 0);
                    break;

                case Handle.BottomLeft:
                    ResizeRect(dx, 0, 0, dy);
                    break;

                case Handle.Bottom:
                    ResizeRect(0, 0, 0, dy);
                    break;

                case Handle.BottomRight:
                    ResizeRect(0, 0, dx, dy);
                    break;

                case Handle.Move:
                    MoveRect(dx, dy);
                    break;
            }
        }

        private void ResizeRect(double dl, double dt, double dr, double db)
        {
            this.rect = RectFromLTRB(startRect.Left + dl, startRect.Top + dt, startRect.Right + dr, startRect.Bottom + db, MinSize);
            rect.Intersect(MaxBounds);
            RectChanged?.Invoke(this, EventArgs.Empty);
        }

        private void MoveRect(double dx, double dy)
        {
            double x = Clamp(startRect.X + dx, MaxBounds.Left, MaxBounds.Right - startRect.Width);
            double y = Clamp(startRect.Y + dy, MaxBounds.Top, MaxBounds.Bottom - startRect.Height);
            this.rect = new Rect(x, y, startRect.Width, startRect.Height);
            RectChanged?.Invoke(this, EventArgs.Empty);
        }

        private double Clamp(double v, double min, double max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        private Handle HandleAt(Point pt)
        {
            if (Matches(pt, rect.Left, rect.Top))
            {
                return Handle.TopLeft;
            }

            if (Matches(pt, rect.Right, rect.Top))
            {
                return Handle.TopRight;
            }

            if (Matches(pt, rect.Left, rect.Bottom))
            {
                return Handle.BottomLeft;
            }

            if (Matches(pt, rect.Right, rect.Bottom))
            {
                return Handle.BottomRight;
            }

            if (Matches(pt.X, rect.Left))
            {
                return Handle.Left;
            }

            if (Matches(pt.X, rect.Right))
            {
                return Handle.Right;
            }

            if (Matches(pt.Y, rect.Top))
            {
                return Handle.Top;
            }

            if (Matches(pt.Y, rect.Bottom))
            {
                return Handle.Bottom;
            }

            if (rect.Contains(pt))
            {
                return Handle.Move;
            }

            return Handle.Disabled;
        }

        private bool Matches(Point pt, double x, double y)
        {
            return Matches(pt.X, x) && Matches(pt.Y, y);
        }

        private bool Matches(double a, double b)
        {
            return Math.Abs(a - b) <= HandleTolerance;
        }

        private enum Handle
        {
            Disabled,
            TopLeft,
            Top,
            TopRight,
            Left,
            Right,
            BottomLeft,
            Bottom,
            BottomRight,
            Move
        }
    }
}
