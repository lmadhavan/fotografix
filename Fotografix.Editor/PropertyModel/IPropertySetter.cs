namespace Fotografix.Editor.PropertyModel
{
    public interface IPropertySetter
    {
        void SetProperty(object target, string propertyName, object newValue);
    }
}
