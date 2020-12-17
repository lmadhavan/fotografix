using System;
using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Editor.Crop
{
    public sealed class CropCommand : ICommand, IEquatable<CropCommand>
    {
        public CropCommand(Image image, Rectangle rectangle)
        {
            this.Image = image;
            this.Rectangle = rectangle;
        }

        public Image Image { get; }
        public Rectangle Rectangle { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as CropCommand);
        }

        public bool Equals(CropCommand other)
        {
            return other != null &&
                   EqualityComparer<Image>.Default.Equals(Image, other.Image) &&
                   EqualityComparer<Rectangle>.Default.Equals(Rectangle, other.Rectangle);
        }

        public override int GetHashCode()
        {
            int hashCode = 1976325080;
            hashCode = hashCode * -1521134295 + EqualityComparer<Image>.Default.GetHashCode(Image);
            hashCode = hashCode * -1521134295 + Rectangle.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CropCommand left, CropCommand right)
        {
            return EqualityComparer<CropCommand>.Default.Equals(left, right);
        }

        public static bool operator !=(CropCommand left, CropCommand right)
        {
            return !(left == right);
        }
    }
}