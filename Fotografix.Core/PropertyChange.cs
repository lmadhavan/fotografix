using System;
using System.Collections.Generic;

namespace Fotografix
{
    public sealed class PropertyChange : IChange, IEquatable<PropertyChange>
    {
        private readonly object target;
        private readonly string propertyName;
        private readonly object oldValue;
        private readonly object newValue;

        public PropertyChange(object target, string propertyName, object oldValue, object newValue)
        {
            this.target = target;
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public override string ToString()
        {
            return $"PropertyChange [{target.GetType().Name}.{propertyName}, oldValue={oldValue}, newValue={newValue}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as PropertyChange);
        }

        public bool Equals(PropertyChange other)
        {
            return other != null &&
                   EqualityComparer<object>.Default.Equals(this.target, other.target) &&
                   this.propertyName == other.propertyName &&
                   EqualityComparer<object>.Default.Equals(this.oldValue, other.oldValue) &&
                   EqualityComparer<object>.Default.Equals(this.newValue, other.newValue);
        }

        public override int GetHashCode()
        {
            int hashCode = 1962993936;
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(this.target);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.propertyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(this.oldValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(this.newValue);
            return hashCode;
        }

        public static bool operator ==(PropertyChange left, PropertyChange right)
        {
            return EqualityComparer<PropertyChange>.Default.Equals(left, right);
        }

        public static bool operator !=(PropertyChange left, PropertyChange right)
        {
            return !(left == right);
        }

        #endregion
    }
}