using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fotografix
{
    public abstract class ImageElement : INotifyPropertyChanged
    {
        private readonly Dictionary<object, object> userProperties = new Dictionary<object, object>();

        public ImageElement Parent { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler UserPropertyChanged;
        public event EventHandler<ContentChangedEventArgs> ContentChanged;

        public abstract bool Accept(ImageElementVisitor visitor);

        public T GetUserProperty<T>(UserProperty<T> property)
        {
            if (userProperties.TryGetValue(property, out var value))
            {
                return (T)value;
            }

            return default;
        }

        public void SetUserProperty<T>(UserProperty<T> property, T value)
        {
            T oldValue = GetUserProperty(property);

            if (!EqualityComparer<T>.Default.Equals(oldValue, value))
            {
                userProperties[property] = value;
                UserPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property.Id));
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseContentChanged(IChange change)
        {
            ContentChangedEventArgs args = new ContentChangedEventArgs(change);
            ImageElement element = this;

            while (element != null)
            {
                element.ContentChanged?.Invoke(this, args);
                element = element.Parent;
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                SetField(ref field, value, propertyName);
                return true;
            }

            return false;
        }

        protected bool SetChild<T>(ref T field, T value, [CallerMemberName] string propertyName = "") where T : ImageElement
        {
            if (field == value)
            {
                return false;
            }

            if (value != null)
            {
                AddChild(value);
            }

            if (field != null)
            {
                RemoveChild(field);
            }

            SetField(ref field, value, propertyName);
            return true;
        }

        protected void AddChild(ImageElement element)
        {
            if (element.Parent != null)
            {
                throw new InvalidOperationException($"{element} is already attached to another element ({element.Parent})");
            }

            element.Parent = this;
        }

        protected void RemoveChild(ImageElement element)
        {
            if (element.Parent != this)
            {
                throw new InvalidOperationException($"{element} is not a child of this element");
            }

            element.Parent = null;
        }

        private void SetField<T>(ref T field, T value, string propertyName)
        {
            T oldValue = field;
            field = value;

            RaisePropertyChanged(propertyName);
            RaiseContentChanged(new PropertyChange(this, propertyName, oldValue, value));
        }
    }
}
