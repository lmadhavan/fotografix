using System.Reflection;

namespace Fotografix.History
{
    public sealed class PropertyChange : Change
    {
        private readonly object target;
        private readonly PropertyInfo propertyInfo;
        private readonly object oldValue;
        private object newValue;

        public PropertyChange(object target, string propertyName, object oldValue, object newValue)
        {
            this.target = target;
            this.propertyInfo = target.GetType().GetProperty(propertyName);
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public override void Redo()
        {
            propertyInfo.SetValue(target, newValue);
        }

        public override void Undo()
        {
            propertyInfo.SetValue(target, oldValue);
        }

        public override bool TryMergeInto(Change change)
        {
            if (change is PropertyChange pc &&
                pc.target == this.target &&
                pc.propertyInfo == this.propertyInfo)
            {
                pc.newValue = this.newValue;
                return true;
            }

            return false;
        }
    }
}
