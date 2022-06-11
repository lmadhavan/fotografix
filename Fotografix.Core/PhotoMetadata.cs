using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;

namespace Fotografix
{
    public sealed class PhotoMetadata
    {
        public const string FileNameKey = "System.FileName";
        public const string CaptureDateKey = "System.Photo.DateTaken";
        public const string CameraManufacturerKey = "System.Photo.CameraManufacturer";
        public const string CameraModelKey = "System.Photo.CameraModel";
        public const string ImageWidthKey = "System.Image.HorizontalSize";
        public const string ImageHeightKey = "System.Image.VerticalSize";
        public const string FocalLengthKey = "System.Photo.FocalLength";
        public const string FNumberKey = "System.Photo.FNumber";
        public const string ExposureTimeKey = "System.Photo.ExposureTime";
        public const string ISOSpeedKey = "System.Photo.ISOSpeed";

        public static readonly PhotoMetadata Empty = new PhotoMetadata(new Dictionary<string, object>());

        private static readonly IEnumerable<string> MetadataProperties = new string[]
        {
            FileNameKey,
            CaptureDateKey,
            CameraManufacturerKey,
            CameraModelKey,
            ImageWidthKey,
            ImageHeightKey,
            FocalLengthKey,
            FNumberKey,
            ExposureTimeKey,
            ISOSpeedKey
        };

        private readonly IDictionary<string, object> properties;

        public PhotoMetadata(IDictionary<string, object> properties)
        {
            this.properties = properties;
        }

        public string FileName => GetString(FileNameKey);
        public DateTimeOffset? CaptureDate => GetProperty<DateTimeOffset>(CaptureDateKey);

        public string CameraManufacturer => GetString(CameraManufacturerKey);
        public string CameraModel => GetString(CameraModelKey);

        public int? ImageWidth => (int?)GetProperty<uint>(ImageWidthKey);
        public int? ImageHeight => (int?)GetProperty<uint>(ImageHeightKey);

        public double? FocalLength => GetProperty<double>(FocalLengthKey);
        public double? FNumber => GetProperty<double>(FNumberKey);
        public double? ExposureTime => GetProperty<double>(ExposureTimeKey);
        public int? ISOSpeed => GetProperty<ushort>(ISOSpeedKey);

        public static async Task<PhotoMetadata> CreateFromStorageItemAsync(IStorageItemExtraProperties storageItemExtraProperties)
        {
            return new PhotoMetadata(await storageItemExtraProperties.RetrievePropertiesAsync(MetadataProperties));
        }

        private string GetString(string key)
        {
            if (properties.TryGetValue(key, out object value))
            {
                return (string)value;
            }

            return null;
        }

        private T? GetProperty<T>(string key) where T : struct
        {
            if (properties.TryGetValue(key, out object value))
            {
                return (T)value;
            }

            return null;
        }
    }
}