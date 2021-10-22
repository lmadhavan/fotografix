using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Fotografix
{
    public sealed class ColorRangeAdjustment : IReadOnlyDictionary<ColorRange, HslValue>
    {
        private static readonly ColorRange[] Keys = (ColorRange[])Enum.GetValues(typeof(ColorRange));

        private readonly HslValue[] values = new HslValue[Keys.Length];
        internal readonly Vector3[] vectors = new Vector3[Keys.Length];

        public ColorRangeAdjustment()
        {
            for (int i = 0; i < values.Length; i++)
            {
                BindColorRange(i);
            }

            this.HueView = new ColorRangeView(this, HslValue.HueComponentIndex);
            this.SaturationView = new ColorRangeView(this, HslValue.SaturationComponentIndex);
            this.LuminanceView = new ColorRangeView(this, HslValue.LuminanceComponentIndex);
        }

        public ColorRangeAdjustment(IEnumerable<KeyValuePair<ColorRange, HslValue>> values) : this()
        {
            foreach (var kv in values)
            {
                this[kv.Key].SetValue(kv.Value);
            }
        }

        public HslValue this[ColorRange colorRange] => values[(int)colorRange];

        [JsonIgnore] public ColorRangeView HueView { get; }
        [JsonIgnore] public ColorRangeView SaturationView { get; }
        [JsonIgnore] public ColorRangeView LuminanceView { get; }

        public event EventHandler Changed;

        private void BindColorRange(int i)
        {
            values[i] = new HslValue();
            values[i].PropertyChanged += (s, e) =>
            {
                vectors[i] = values[i].ToVector3();
                RaiseChanged();
            };
        }

        private void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        #region IReadOnlyDictionary

        int IReadOnlyCollection<KeyValuePair<ColorRange, HslValue>>.Count => Keys.Length;
        IEnumerable<ColorRange> IReadOnlyDictionary<ColorRange, HslValue>.Keys => Keys;
        IEnumerable<HslValue> IReadOnlyDictionary<ColorRange, HslValue>.Values => values;
        bool IReadOnlyDictionary<ColorRange, HslValue>.ContainsKey(ColorRange key) => true;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public IEnumerator<KeyValuePair<ColorRange, HslValue>> GetEnumerator()
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                if (values[i].IsZero)
                {
                    continue;
                }

                yield return new KeyValuePair<ColorRange, HslValue>(Keys[i], values[i]);
            }
        }

        bool IReadOnlyDictionary<ColorRange, HslValue>.TryGetValue(ColorRange key, out HslValue value)
        {
            value = this[key];
            return true;
        }

        #endregion
    }
}
