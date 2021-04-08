using System.Reflection;

namespace Fotografix
{
    public sealed record PropertyChange(object Target, string PropertyName, object OldValue, object NewValue) : IMergeableChange
    {
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
                result = this with { OldValue = pc.OldValue };
                return true;
            }

            result = null;
            return false;
        }
    }
}