namespace Fotografix
{
    public sealed class PhotoMetadataViewModel
    {
        private readonly PhotoMetadata metadata;

        public PhotoMetadataViewModel(PhotoMetadata metadata)
        {
            this.metadata = metadata;
        }

        public string Camera
        {
            get
            {
                var manufacturer = metadata.CameraManufacturer ?? "";
                var model = metadata.CameraModel ?? "";

                return model.StartsWith(manufacturer) ? model : manufacturer + " " + model;
            }
        }

        public string FileName => metadata.FileName;
        public string CaptureDate => Format(metadata.CaptureDate, "{0:G}");
        public string Dimensions => $"{metadata.ImageWidth}×{metadata.ImageHeight}";
        public string FocalLength => Format(metadata.FocalLength, "{0:0}mm");
        public string FNumber => Format(metadata.FNumber, "ƒ/{0:F1}");
        public string ISOSpeed => Format(metadata.ISOSpeed, "ISO {0}");

        public string ExposureTime
        {
            get
            {
                var t = metadata.ExposureTime;

                if (t == null)
                {
                    return "";
                }

                if (t < 1)
                {
                    return string.Format("1/{0:F0} sec", 1 / t);
                }

                return string.Format("{0:F0} sec", t);
            }
        }


        private string Format<T>(T? value, string format) where T : struct
        {
            return value.HasValue ? string.Format(format, value.Value) : "";
        }
    }
}
