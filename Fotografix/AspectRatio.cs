using System.Collections.Generic;

namespace Fotografix
{
    public sealed class AspectRatio
    {
        public static readonly AspectRatio Unconstrained = new AspectRatio { DisplayName = "Unconstrained" };

        public static readonly IReadOnlyList<AspectRatio> StandardRatios = new List<AspectRatio>
        {
            new AspectRatio(1, 1),
            new AspectRatio(4, 3),
            new AspectRatio(3, 2),
            new AspectRatio(16, 9)
        };

        private AspectRatio()
        {
        }

        public AspectRatio(double num, double den, string displayName = null)
        {
            this.DisplayName = displayName ?? $"{num}:{den}";
            this.Value = num / den;
            this.InverseValue = den / num;
        }

        public string DisplayName { get; private set; }
        public double? Value { get; private set; }
        public double? InverseValue { get; private set; }
    }
}
