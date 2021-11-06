using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fotografix
{
    [TestClass]
    public class ExceptionMessageConverterTest
    {
        [TestMethod]
        public void ReturnsExceptionMessage()
        {
            var converter = new ExceptionMessageConverter();

            var exception = new Exception("message");

            Assert.AreEqual("message", converter.Convert(exception, null, null, null));
        }

        [TestMethod]
        public void UnwrapsAggregateException()
        {
            var converter = new ExceptionMessageConverter();

            var exception = new AggregateException(new Exception("message1"), new Exception("message2"));

            Assert.AreEqual("message1\nmessage2", converter.Convert(exception, null, null, null));
        }
    }
}
