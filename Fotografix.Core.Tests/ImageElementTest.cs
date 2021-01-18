using NUnit.Framework;
using System;
using System.ComponentModel;

namespace Fotografix
{
    [TestFixture]
    public class ImageElementTest
    {
        [Test]
        public void RaisesPropertyChangedEvent()
        {
            FakeImageElement element = new FakeImageElement();

            PropertyChangedEventArgs propertyChangedEvent = null;
            element.PropertyChanged += (s, e) => propertyChangedEvent = e;

            element.Property = 10;

            Assert.That(propertyChangedEvent.PropertyName, Is.EqualTo(nameof(FakeImageElement.Property)));
        }

        [Test]
        public void RaisesContentChangedEvent()
        {
            FakeImageElement element = new FakeImageElement();

            ContentChangedEventArgs contentChangedEvent = null;
            element.ContentChanged += (s, e) => contentChangedEvent = e;

            element.Property = 10;

            Assert.That(contentChangedEvent.Change, Is.EqualTo(new PropertyChange(element, nameof(FakeImageElement.Property), 0, 10)));
        }

        [Test]
        public void RaisesUserPropertyChangedEvent()
        {
            FakeImageElement element = new FakeImageElement();
            UserPropertyKey<string> key = new UserPropertyKey<string>();

            UserPropertyChangedEventArgs userPropertyChangedEvent = null;
            element.UserPropertyChanged += (s, e) => userPropertyChangedEvent = e;

            element.SetUserProperty(key, "foo");

            Assert.That(userPropertyChangedEvent.Key, Is.EqualTo(key));
        }

        [Test]
        public void PropagatesContentChangeEventToParent()
        {
            FakeImageElement parent = new FakeImageElement();
            FakeImageElement child = new FakeImageElement();
            parent.Child = child;

            ContentChangedEventArgs contentChangedEvent = null;
            parent.ContentChanged += (s, e) => contentChangedEvent = e;

            child.Property = 10;

            Assert.That(contentChangedEvent, Is.Not.Null);
        }

        [Test]
        public void ThrowsExceptionWhenAddingChildOfAnotherElement()
        {
            FakeImageElement parent1 = new FakeImageElement();
            FakeImageElement parent2 = new FakeImageElement();
            FakeImageElement child = new FakeImageElement();

            parent1.Child = child;

            Assert.Throws<InvalidOperationException>(() => parent2.Child = child);
            Assert.That(child.Parent, Is.EqualTo(parent1));
        }

        [Test]
        public void AllowsSettingChildToExistingValue()
        {
            FakeImageElement parent = new FakeImageElement();
            FakeImageElement child = new FakeImageElement();

            parent.Child = child;
            parent.Child = child;

            Assert.Pass();
        }

        private class FakeImageElement : ImageElement
        {
            private int property;
            private FakeImageElement child;

            public int Property
            {
                get => property;
                set => SetProperty(ref property, value);
            }

            public FakeImageElement Child
            {
                get => child;
                set => SetChild(ref child, value);
            }

            public override bool Accept(ImageElementVisitor visitor) => true;
        }
    }
}
