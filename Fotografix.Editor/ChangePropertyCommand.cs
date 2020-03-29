using System.Reflection;

namespace Fotografix.Editor
{
    public sealed class ChangePropertyCommand : Command
    {
        private readonly object target;
        private readonly PropertyInfo propertyInfo;
        private object oldValue;
        private object newValue;

        public ChangePropertyCommand(object target, string propertyName, object newValue)
        {
            this.target = target;
            this.propertyInfo = target.GetType().GetProperty(propertyName);
            this.newValue = newValue;
        }

        private object CurrentValue => propertyInfo.GetValue(target);
        
        public override bool IsEffective => !Equals(CurrentValue, newValue);

        public override void Execute()
        {
            this.oldValue = CurrentValue;
            propertyInfo.SetValue(target, newValue);
        }

        public override void Undo()
        {
            propertyInfo.SetValue(target, oldValue);
        }

        public override bool TryMergeInto(Command command)
        {
            if (command is ChangePropertyCommand cpc &&
                cpc.target == this.target &&
                cpc.propertyInfo == this.propertyInfo)
            {
                cpc.newValue = this.newValue;
                return true;
            }

            return false;
        }
    }
}
