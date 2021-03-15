using Fotografix.Drawing;
using System;
using System.Collections.Generic;

namespace Fotografix.Editor.Drawing
{
    public sealed class DrawCommand : ICommand, IEquatable<DrawCommand>
    {
        public DrawCommand(BitmapLayer bitmap, IDrawable drawable)
        {
            this.Layer = bitmap;
            this.Drawable = drawable;
        }

        public BitmapLayer Layer { get; }
        public IDrawable Drawable { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as DrawCommand);
        }

        public bool Equals(DrawCommand other)
        {
            return other != null &&
                   EqualityComparer<BitmapLayer>.Default.Equals(Layer, other.Layer) &&
                   EqualityComparer<IDrawable>.Default.Equals(Drawable, other.Drawable);
        }

        public override int GetHashCode()
        {
            int hashCode = 1468620837;
            hashCode = hashCode * -1521134295 + EqualityComparer<BitmapLayer>.Default.GetHashCode(Layer);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDrawable>.Default.GetHashCode(Drawable);
            return hashCode;
        }

        public static bool operator ==(DrawCommand left, DrawCommand right)
        {
            return EqualityComparer<DrawCommand>.Default.Equals(left, right);
        }

        public static bool operator !=(DrawCommand left, DrawCommand right)
        {
            return !(left == right);
        }
    }
}
