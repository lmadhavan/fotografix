using System.ComponentModel;

namespace Fotografix
{
    public interface IPhotoAdjustment : INotifyPropertyChanged
    {
        #region Light
        float Exposure { get; set; }
        float Contrast { get; set; }
        float Highlights { get; set; }
        float Shadows { get; set; }
        float Whites { get; set; }
        float Blacks { get; set; }
        #endregion

        #region Color
        float Temperature { get; set; }
        float Tint { get; set; }
        float Vibrance { get; set; }
        float Saturation { get; set; }
        ColorRangeAdjustment ColorRanges { get; set; }
        #endregion

        #region Detail
        float Clarity { get; set; }
        float Sharpness { get; set; }
        #endregion
    }
}