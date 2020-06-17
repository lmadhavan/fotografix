namespace Fotografix.Editor.PropertyModel
{
    public sealed class UndoablePropertySetter : IPropertySetter
    {
        private readonly History history;

        public UndoablePropertySetter(History history)
        {
            this.history = history;
        }

        public void SetProperty(object target, string propertyName, object newValue)
        {
            ChangePropertyCommand command = new ChangePropertyCommand(target, propertyName, newValue);
            if (command.IsEffective)
            {
                command.Execute();
                history.Add(command);
            }
        }
    }
}
