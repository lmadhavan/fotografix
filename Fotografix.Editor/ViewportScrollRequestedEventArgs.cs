using System;
using System.Drawing;

namespace Fotografix.Editor
{
    public class ViewportScrollRequestedEventArgs : EventArgs
    {
        public ViewportScrollRequestedEventArgs(PointF scrollDelta)
        {
            this.ScrollDelta = scrollDelta;
        }

        public PointF ScrollDelta { get; }
    }
}