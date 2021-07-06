﻿using Fotografix.Editor.ChangeTracking;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor
{
    [TestFixture]
    public class DocumentTest
    {
        private Mock<IAppendableHistory> history;

        private Image image;
        private Document document;
        private int contentChangedCount;

        [SetUp]
        public void SetUp()
        {
            this.history = new Mock<IAppendableHistory>();

            this.image = new Image(new Size(10, 10));
            this.document = new Document(image, history.Object);

            this.contentChangedCount = 0;
            document.ContentChanged += (s, e) => this.contentChangedCount++;
        }

        [Test]
        public void TracksChangesToImage()
        {
            ProduceChange(image);

            history.Verify(h => h.Add(It.IsAny<IChange>()));
            Assert.IsTrue(document.IsDirty, "Dirty");
            Assert.That(contentChangedCount, Is.EqualTo(1), "Content changed");
        }

        [Test]
        public void CanGroupChanges()
        {
            using (document.BeginChangeGroup())
            {
                ProduceChange(image);
                ProduceChange(image);

                history.VerifyNoOtherCalls();
            }

            history.Verify(h => h.Add(It.IsAny<CompositeChange>()), Times.Once);
            Assert.IsTrue(document.IsDirty, "Dirty");
            Assert.That(contentChangedCount, Is.EqualTo(1), "Content changed");
        }

        [Test]
        public void DoesNotAddEmptyChangeGroupsToHistory()
        {
            using (document.BeginChangeGroup())
            {
            }

            history.VerifyNoOtherCalls();
            Assert.IsFalse(document.IsDirty, "Dirty");
            Assert.That(contentChangedCount, Is.EqualTo(0), "Content changed");
        }

        [Test]
        public void IsDirtyOnUndo()
        {
            document.Undo();

            history.Verify(h => h.Undo());
            Assert.IsTrue(document.IsDirty, "Dirty");
            Assert.That(contentChangedCount, Is.EqualTo(1), "Content changed");
        }

        [Test]
        public void IgnoresAdditionalChangesWhenUndoing()
        {
            history.Setup(h => h.Undo()).Callback(() => ProduceChange(image));

            document.Undo();

            history.Verify(h => h.Add(It.IsAny<IChange>()), Times.Never);
            Assert.That(contentChangedCount, Is.EqualTo(1), "Content changed");
        }

        [Test]
        public void IsDirtyOnRedo()
        {
            document.Redo();

            history.Verify(h => h.Redo());
            Assert.IsTrue(document.IsDirty, "Dirty");
            Assert.That(contentChangedCount, Is.EqualTo(1), "Content changed");
        }

        [Test]
        public void IgnoresAdditionalChangesWhenRedoing()
        {
            history.Setup(h => h.Redo()).Callback(() => ProduceChange(image));

            document.Redo();

            history.Verify(h => h.Add(It.IsAny<IChange>()), Times.Never);
            Assert.That(contentChangedCount, Is.EqualTo(1), "Content changed");
        }

        private static void ProduceChange(Image image)
        {
            image.Size *= 2;
        }
    }
}
