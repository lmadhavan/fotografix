using System.Drawing;

namespace Fotografix.Editor.Tools
{
    internal sealed class MoveTracker
    {
        private readonly Size offset;

        internal MoveTracker(Point objectPosition, PointerState startState)
        {
            this.offset = new(objectPosition.X - startState.Location.X, objectPosition.Y - startState.Location.Y);
        }

        public Point ObjectPositionAt(PointerState currentState)
        {
            return currentState.Location + offset;
        }
    }
}
