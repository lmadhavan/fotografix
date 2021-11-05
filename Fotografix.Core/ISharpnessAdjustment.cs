using System.ComponentModel;

namespace Fotografix
{
    public interface ISharpnessAdjustment : INotifyPropertyChanged
    {
        float Amount { get; set; }
        float Radius { get; set; }
        float Threshold { get; set; }
    }
}