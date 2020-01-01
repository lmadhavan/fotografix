using System.Reflection;

namespace Fotografix.Editor.Commands
{
    /// <summary>
    /// Changes the value of a specific property of an object.
    /// </summary>
    /// <remarks>
    /// This command produces mergeable changes; successive changes to the same property
    /// of the same object can be merged into a single change.
    /// </remarks>
    public sealed class ChangePropertyCommand : ICommand
    {
        private readonly object target;
        private readonly string propertyName;
        private readonly object newValue;

        public ChangePropertyCommand(object target, string propertyName, object newValue)
        {
            this.target = target;
            this.propertyName = propertyName;
            this.newValue = newValue;
        }

        public IChange PrepareChange()
        {
            PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);

            object oldValue = propertyInfo.GetValue(target);
            if (object.Equals(oldValue, newValue))
            {
                return null;
            }

            return new PropertyChange(target, propertyInfo, oldValue, newValue);
        }

        private sealed class PropertyChange : IMergeableChange
        {
            private readonly object target;
            private readonly PropertyInfo propertyInfo;
            private readonly object oldValue;
            private object newValue;

            public PropertyChange(object target, PropertyInfo propertyInfo, object oldValue, object newValue)
            {
                this.target = target;
                this.propertyInfo = propertyInfo;
                this.oldValue = oldValue;
                this.newValue = newValue;
            }

            public void Apply()
            {
                propertyInfo.SetValue(target, newValue);
            }

            public void Undo()
            {
                propertyInfo.SetValue(target, oldValue);
            }

            public bool TryMergeInto(IChange change)
            {
                if (change is PropertyChange propertyChange &&
                    propertyChange.target == this.target &&
                    propertyChange.propertyInfo == this.propertyInfo)
                {
                    propertyChange.newValue = this.newValue;
                    return true;
                }

                return false;
            }
        }
    }
}
