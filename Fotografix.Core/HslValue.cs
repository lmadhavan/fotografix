using System.Numerics;

namespace Fotografix
{
    public sealed class HslValue : NotifyPropertyChangedBase
    {
        public const int HueComponentIndex = 0;
        public const int SaturationComponentIndex = 1;
        public const int LuminanceComponentIndex = 2;

        private static readonly string[] ComponentNames = new string[] { nameof(Hue), nameof(Saturation), nameof(Luminance) };

        private readonly float[] components = new float[3];

        public float Hue
        {
            get => components[0];
            set => SetProperty(ref components[0], value);
        }

        public float Saturation
        {
            get => components[1];
            set => SetProperty(ref components[1], value);
        }

        public float Luminance
        {
            get => components[2];
            set => SetProperty(ref components[2], value);
        }

        public float this[int index]
        {
            get => components[index];
            set => SetProperty(ref components[index], value, ComponentNames[index]);
        }

        public bool IsZero => components[0] == 0 && components[1] == 0 && components[2] == 0;

        public void SetValue(HslValue other)
        {
            for (int i = 0; i < components.Length; i++)
            {
                this[i] = other[i];
            }
        }

        public Vector3 ToVector3()
        {
            return new Vector3(components[0], components[1], components[2]);
        }
    }
}
