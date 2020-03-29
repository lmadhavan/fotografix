using Fotografix.Editor;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fotografix.UI
{
    /// <summary>
    /// Provides a base class for a view model that supports editing of object properties.
    /// </summary>
    /// <typeparam name="T">The type of object being edited.</typeparam>
    /// <remarks>
    /// By default, property change notifications from the target object are propagated to clients
    /// of the view model. To support this behavior, derived classes should define properties with
    /// the same names as those being edited in the target object. Derived classes may also override
    /// <see cref="OnTargetPropertyChanged(object, PropertyChangedEventArgs)"/> to customize this
    /// behavior.
    /// </remarks>
    public abstract class PropertyEditorViewModelBase<T> : NotifyPropertyChangedBase, IDisposable where T : INotifyPropertyChanged
    {
        protected PropertyEditorViewModelBase(T target, ICommandService commandService)
        {
            this.Target = target;
            Target.PropertyChanged += OnTargetPropertyChanged;

            this.CommandService = commandService;
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
        /// Gets the <see cref="ICommandService"/> used to execute commands.
        /// </summary>
        protected ICommandService CommandService { get; }

        /// <summary>
        /// Sets the specified property of the target object to the specified value. This method
        /// executes a <see cref="ChangePropertyCommand"/> so that the change is added to the history.
        /// </summary>
        protected void SetTargetProperty(object newValue, [CallerMemberName] string propertyName = "")
        {
            CommandService.Execute(new ChangePropertyCommand(Target, propertyName, newValue));
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
