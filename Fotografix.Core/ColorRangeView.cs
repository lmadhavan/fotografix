using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fotografix
{
    public sealed class ColorRangeView : NotifyPropertyChangedBase
    {
        private readonly IReadOnlyDictionary<ColorRange, HslValue> values;
        private readonly int componentIndex;

        public ColorRangeView(IReadOnlyDictionary<ColorRange, HslValue> values, int componentIndex)
        {
            this.values = values;
            this.componentIndex = componentIndex;
        }

        public float Red
        {
            get => GetValue(ColorRange.Red);
            set => SetValue(ColorRange.Red, value);
        }

        public float Yellow
        {
            get => GetValue(ColorRange.Yellow);
            set => SetValue(ColorRange.Yellow, value);
        }

        public float Green
        {
            get => GetValue(ColorRange.Green);
            set => SetValue(ColorRange.Green, value);
        }

        public float Cyan
        {
            get => GetValue(ColorRange.Cyan);
            set => SetValue(ColorRange.Cyan, value);
        }

        public float Blue
        {
            get => GetValue(ColorRange.Blue);
            set => SetValue(ColorRange.Blue, value);
        }

        public float Magenta
        {
            get => GetValue(ColorRange.Magenta);
            set => SetValue(ColorRange.Magenta, value);
        }

        public void Reset()
        {
            Red = Yellow = Green = Cyan = Blue = Magenta = 0;
        }

        private float GetValue(ColorRange colorRange)
        {
            return values[colorRange][componentIndex];
        }

        private void SetValue(ColorRange colorRange, float value, [CallerMemberName] string propertyName = "")
        {
            values[colorRange][componentIndex] = value;
            RaisePropertyChanged(propertyName);
        }
    }
}