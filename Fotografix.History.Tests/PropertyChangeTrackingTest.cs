using Moq;
using NUnit.Framework;

namespace Fotografix.History
{
    [TestFixture]
    public class PropertyChangeTrackingTest
    {
        private Mock<IChangeTracker> changeTracker;

        [SetUp]
        public void SetUp()
        {
            this.changeTracker = new Mock<IChangeTracker>();
        }

        [Test]
        public void AddsPropertyChangeToTracker()
        {
            var obj = new TestObject() { Value = 2 };
            var trackedObj = new ChangeTrackingTestObject(obj, changeTracker.Object);
            
            trackedObj.Value = 5;

            changeTracker.Verify(t => t.Add(It.IsAny<PropertyChange>()));
        }

        [Test]
        public void IgnoresPropertyChangeWhenOldAndNewValuesAreEqual()
        {
            var obj = new TestObject() { Value = 2 };
            var trackedObj = new ChangeTrackingTestObject(obj, changeTracker.Object);

            trackedObj.Value = 2;

            changeTracker.VerifyNoOtherCalls();
        }

        private class TestObject : NotifyPropertyChangedBase
        {
            public int Value { get; set; }
        }

        private class ChangeTrackingTestObject : PropertyChangeTrackingBase<TestObject>
        {
            public ChangeTrackingTestObject(TestObject target, IChangeTracker changeTracker) : base(target, changeTracker)
            {
            }

            public int Value
            {
                get => Target.Value;
                set => AddPropertyChange(Target.Value, value);
            }
        }
    }
}
