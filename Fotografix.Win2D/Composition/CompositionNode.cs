using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition
{
    /// <summary>
    /// Represents a node in a Win2D composition graph.
    /// </summary>
    internal abstract class CompositionNode
    {
        private ICanvasImage output;

        /// <summary>
        /// Gets the output image of the node.
        /// </summary>
        public ICanvasImage Output
        {
            get
            {
                return output;
            }

            protected set
            {
                if (output != value)
                {
                    this.output = value;
                    OutputChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the value of the <see cref="Output"/> property has changed.
        /// </summary>
        public event EventHandler OutputChanged;
    }
}
