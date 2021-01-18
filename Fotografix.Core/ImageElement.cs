using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fotografix
{
    public abstract class ImageElement : INotifyPropertyChanged
    {
        private readonly Dictionary<IUserPropertyKey, object> userProperties = new Dictionary<IUserPropertyKey, object>();

        public ImageElement Parent { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<UserPropertyChangedEventArgs> UserPropertyChanged;
        public event EventHandler<ContentChangedEventArgs> ContentChanged;

        public abstract bool Accept(ImageElementVisitor visitor);

        public T GetUserProperty<T>(UserPropertyKey<T> key)
        {
            if (userProperties.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return default;
        }

        public void SetUserProperty<T>(UserPropertyKey<T> key, T value)
        {
            T oldValue = GetUserProperty(key);

            if (!EqualityComparer<T>.Default.Equals(oldValue, value))
            {
                userProperties[key] = value;
                UserPropertyChanged?.Invoke(this, new UserPropertyChangedEventArgs(key));
            }
        }

        protected void RaiseContentChanged(Change change)
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

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            RaiseContentChanged(new PropertyChange(this, propertyName, oldValue, value));
        }
    }
}
