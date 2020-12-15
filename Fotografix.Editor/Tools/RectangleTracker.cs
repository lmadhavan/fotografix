using System;
using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class RectangleTracker : IPointerEventListener
    {
        private static readonly Dictionary<Handle, ToolCursor> CursorDictionary = new Dictionary<Handle, ToolCursor>
        {
            [Handle.Disabled] = ToolCursor.Disabled,
            [Handle.TopLeft] = ToolCursor.SizeNorthwestSoutheast,
            [Handle.Top] = ToolCursor.SizeNorthSouth,
            [Handle.TopRight] = ToolCursor.SizeNortheastSouthwest,
            [Handle.Left] = ToolCursor.SizeWestEast,
            [Handle.Right] = ToolCursor.SizeWestEast,
            [Handle.BottomLeft] = ToolCursor.SizeNortheastSouthwest,
            [Handle.Bottom] = ToolCursor.SizeNorthSouth,
            [Handle.BottomRight] = ToolCursor.SizeNorthwestSoutheast,
            [Handle.Move] = ToolCursor.Move,
        };

        private Rectangle rect;
        
        private bool tracking;
        private Handle handle;
        private Point startPoint;
        private Rectangle startRect;

        public RectangleTracker(Rectangle rectangle)
        {
            this.rect = rectangle;
            this.handle = Handle.Disabled;
        }

        public ToolCursor Cursor => CursorDictionary[handle];

        public Rectangle Rectangle => rect;

        public event EventHandler RectangleChanged;

        public int HandleTolerance { get; set; }

        public void PointerPressed(PointerState p)
        {
            if (!tracking)
            {
                this.handle = HandleAt(p.Location);
                this.tracking = true;
                this.startPoint = p.Location;
                this.startRect = rect;
            }
        }

        public void PointerMoved(PointerState p)
        {
            if (tracking)
            {
                Track(p.Location);
            }
            else
            {
                this.handle = HandleAt(p.Location);
            }
        }

        public void PointerReleased(PointerState p)
        {
            this.tracking = false;
        }

        private void Track(Point pt)
        {
            int dx = pt.X - startPoint.X;
            int dy = pt.Y - startPoint.Y;

            switch (handle)
            {
                case Handle.TopLeft:
                    AdjustRectangle(dx, dy, 0, 0);
                    break;

                case Handle.Top:
                    AdjustRectangle(0, dy, 0, 0);
                    break;

                case Handle.TopRight:
                    AdjustRectangle(0, dy, dx, 0);
                    break;

                case Handle.Left:
                    AdjustRectangle(dx, 0, 0, 0);
                    break;

                case Handle.Right:
                    AdjustRectangle(0, 0, dx, 0);
                    break;

                case Handle.BottomLeft:
                    AdjustRectangle(dx, 0, 0, dy);
                    break;

                case Handle.Bottom:
                    AdjustRectangle(0, 0, 0, dy);
                    break;

                case Handle.BottomRight:
                    AdjustRectangle(0, 0, dx, dy);
                    break;

                case Handle.Move:
                    AdjustRectangle(dx, dy, dx, dy);
                    break;
            }
        }

        private void AdjustRectangle(int dl, int dt, int dr, int db)
        {
            this.rect = Rectangle.FromLTRB(startRect.Left + dl, startRect.Top + dt, startRect.Right + dr, startRect.Bottom + db);
            RectangleChanged?.Invoke(this, EventArgs.Empty);
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

        private bool Matches(Point pt, int x, int y)
        {
            return Matches(pt.X, x) && Matches(pt.Y, y);
        }

        private bool Matches(int a, int b)
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