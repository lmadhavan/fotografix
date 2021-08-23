namespace Fotografix.Uwp
{
    public sealed class BoolToObjectConverter : ValueConverter<bool, object>
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public override object Convert(bool value)
        {
            return value ? TrueValue : FalseValue;
        }
    }
}
