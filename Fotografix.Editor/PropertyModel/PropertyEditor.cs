using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fotografix.Editor.PropertyModel
{
    /// <summary>
    /// Provides a base class for implementing property editors.
    /// </summary>
    /// <typeparam name="T">The type of object being edited.</typeparam>
    /// <remarks>
    /// By default, property change notifications from the target object are propagated to clients
    /// of this class. To support this behavior, derived classes should define properties with
    /// the same names as those being edited in the target object. Derived classes may also override
    /// <see cref="OnTargetPropertyChanged(object, PropertyChangedEventArgs)"/> to customize this
    /// behavior.
    /// </remarks>
    public abstract class PropertyEditor<T> : NotifyPropertyChangedBase, IPropertyEditor where T : INotifyPropertyChanged
    {
        protected PropertyEditor(T target, IPropertySetter propertySetter)
        {
            this.Target = target;
            Target.PropertyChanged += OnTargetPropertyChanged;

            this.PropertySetter = propertySetter;
        }

        public virtual void Dispose()
        {
            Target.PropertyChanged -= OnTargetPropertyChanged;
        }

        /// <summary>
        /// Gets the object being edited.
        /// </summary>
        protected T Target { get; }

        /// <summary>
        /// Gets the <see cref="IPropertySetter"/> used to set property values.
        /// </summary>
        protected IPropertySetter PropertySetter { get; }

        /// <summary>
        /// Sets the specified property of the target object to the specified value.
        /// </summary>
        protected void SetTargetProperty(object newValue, [CallerMemberName] string propertyName = "")
        {
            PropertySetter.SetProperty(Target, propertyName, newValue);
        }

        /// <summary>
        /// Called when a property value in the target object has changed.
        /// </summary>
        protected virtual void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }
}
