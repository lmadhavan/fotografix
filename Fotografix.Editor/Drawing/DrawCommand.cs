using Fotografix.Drawing;
using System;
using System.Collections.Generic;

namespace Fotografix.Editor.Drawing
{
    public sealed class DrawCommand : ICommand, IEquatable<DrawCommand>
    {
        public DrawCommand(Bitmap bitmap, IDrawable drawable)
        {
            this.Bitmap = bitmap;
            this.Drawable = drawable;
        }

        public Bitmap Bitmap { get; }
        public IDrawable Drawable { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as DrawCommand);
        }

        public bool Equals(DrawCommand other)
        {
            return other != null &&
                   EqualityComparer<Bitmap>.Default.Equals(Bitmap, other.Bitmap) &&
                   EqualityComparer<IDrawable>.Default.Equals(Drawable, other.Drawable);
        }

        public override int GetHashCode()
        {
            int hashCode = 1468620837;
            hashCode = hashCode * -1521134295 + EqualityComparer<Bitmap>.Default.GetHashCode(Bitmap);
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
