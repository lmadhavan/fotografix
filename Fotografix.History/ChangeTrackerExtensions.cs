using System.Runtime.CompilerServices;

namespace Fotografix.History
{
    public static class ChangeTrackerExtensions
    {
        public static void AddPropertyChange(this IChangeTracker changeTracker, object target, object oldValue, object newValue, [CallerMemberName] string propertyName = "")
        {
            changeTracker.Add(new PropertyChange(target, propertyName, oldValue, newValue));
        }
    }
}
