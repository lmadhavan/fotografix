using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Fotografix
{
    [TestClass]
    public class PhotoMetadataViewModelTest
    {
        private Dictionary<string, object> properties;
        private PhotoMetadataViewModel vm;

        [TestInitialize]
        public void Initialize()
        {
            this.properties = new Dictionary<string, object>();
            this.vm = new PhotoMetadataViewModel(new PhotoMetadata(properties));
        }

        [TestMethod]
        public void CombinesCameraManufacturerAndModel()
        {
            properties[PhotoMetadata.CameraManufacturerKey] = "Apple";
            properties[PhotoMetadata.CameraModelKey] = "iPhone";

            Assert.AreEqual("Apple iPhone", vm.Camera);
        }

        [TestMethod]
        public void DeduplicatesCameraManufacturerAndModel()
        {
            properties[PhotoMetadata.CameraManufacturerKey] = "Canon";
            properties[PhotoMetadata.CameraModelKey] = "Canon EOS REBEL T2i";

            Assert.AreEqual("Canon EOS REBEL T2i", vm.Camera);
        }

        [TestMethod]
        public void FormatsDimensions()
        {
            properties[PhotoMetadata.ImageWidthKey] = 200U;
            properties[PhotoMetadata.ImageHeightKey] = 100U;

            Assert.AreEqual("200×100", vm.Dimensions);
        }

        [TestMethod]
        public void FormatsFocalLength()
        {
            properties[PhotoMetadata.FocalLengthKey] = 35.1;

            Assert.AreEqual("35mm", vm.FocalLength);
        }

        [TestMethod]
        public void FormatsFNumber()
        {
            properties[PhotoMetadata.FNumberKey] = 4.5;

            Assert.AreEqual("ƒ/4.5", vm.FNumber);
        }

        [TestMethod]
        public void FormatsExposureTimeBelowOneSecond()
        {
            properties[PhotoMetadata.ExposureTimeKey] = 0.33;

            Assert.AreEqual("1/3 sec", vm.ExposureTime);
        }

        [TestMethod]
        public void FormatsExposureTimeAboveOneSecond()
        {
            properties[PhotoMetadata.ExposureTimeKey] = 2.0;

            Assert.AreEqual("2 sec", vm.ExposureTime);
        }

        [TestMethod]
        public void FormatsISOSpeed()
        {
            properties[PhotoMetadata.ISOSpeedKey] = (ushort)100;

            Assert.AreEqual("ISO 100", vm.ISOSpeed);
        }

        [TestMethod]
        public void FormatsCaptureDate()
        {
            properties[PhotoMetadata.CaptureDateKey] = (DateTimeOffset)new DateTime(2000, 1, 1, 10, 20, 30);

            Assert.AreEqual("1/1/2000 10:20:30 AM", vm.CaptureDate);
        }
    }
}
