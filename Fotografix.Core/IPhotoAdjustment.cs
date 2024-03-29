﻿using System.ComponentModel;

namespace Fotografix
{
    public interface IPhotoAdjustment : INotifyPropertyChanged
    {
        #region Light
        float Exposure { get; set; }
        float Brightness { get; set; }
        float Contrast { get; set; }
        float Highlights { get; set; }
        float Shadows { get; set; }
        float Whites { get; set; }
        float Blacks { get; set; }
        #endregion

        #region Color
        bool BlackAndWhite { get; set; }
        float Temperature { get; set; }
        float Tint { get; set; }
        float Vibrance { get; set; }
        float Saturation { get; set; }
        ColorRangeAdjustment ColorRanges { get; }
        #endregion

        #region Detail
        float Clarity { get; set; }
        ISharpnessAdjustment Sharpness { get; }
        #endregion

        #region Transform
        CropRect? Crop { get; set; }
        int Rotation { get; set; }
        bool Flip { get; set; }
        float Straighten { get; set; }
        #endregion
    }
}