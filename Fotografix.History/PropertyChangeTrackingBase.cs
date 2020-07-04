using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fotografix.History
{
    public abstract class PropertyChangeTrackingBase<T> : INotifyPropertyChanged where T : INotifyPropertyChanged
    {
        protected PropertyChangeTrackingBase(T target, IChangeTracker changeTracker)
        {
            this.Target = target;
            this.ChangeTracker = changeTracker;
        }

        protected T Target { get; }
        protected IChangeTracker ChangeTracker { get; }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Target.PropertyChanged += value;
            remove => Target.PropertyChanged -= value;
        }

        protected void AddPropertyChange(object oldValue, object newValue, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(oldValue, newValue))
            {
                ChangeTracker.Add(new PropertyChange(Target, propertyName, oldValue, newValue));
            }
        }
    }
}
