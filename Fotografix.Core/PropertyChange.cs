using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fotografix
{
    public sealed class PropertyChange : IMergeableChange, IEquatable<PropertyChange>
    {
        public PropertyChange(object target, string propertyName, object oldValue, object newValue)
        {
            this.Target = target;
            this.PropertyName = propertyName;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public object Target { get; }
        public string PropertyName { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        private PropertyInfo PropertyInfo => Target.GetType().GetProperty(PropertyName);

        public void Undo()
        {
            PropertyInfo.SetValue(Target, OldValue);
        }

        public void Redo()
        {
            PropertyInfo.SetValue(Target, NewValue);
        }

        public bool TryMergeWith(IChange previous, out IChange result)
        {
            if (previous is PropertyChange pc &&
                pc.Target == this.Target &&
                pc.PropertyName == this.PropertyName)
            {
                result = new PropertyChange(Target, PropertyName, pc.OldValue, this.NewValue);
                return true;
            }

            result = null;
            return false;
        }

        public override string ToString()
        {
            return $"PropertyChange [{Target.GetType().Name}.{PropertyName}, oldValue={OldValue}, newValue={NewValue}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as PropertyChange);
        }

        public bool Equals(PropertyChange other)
        {
            return other != null &&
                   EqualityComparer<object>.Default.Equals(this.Target, other.Target) &&
                   this.PropertyName == other.PropertyName &&
                   EqualityComparer<object>.Default.Equals(this.OldValue, other.OldValue) &&
                   EqualityComparer<object>.Default.Equals(this.NewValue, other.NewValue);
        }

        public override int GetHashCode()
        {
            int hashCode = 1962993936;
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(this.Target);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PropertyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(this.OldValue);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(this.NewValue);
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