using Fotografix.Editor.ChangeTracking;
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

        [Test]
        public void ActivatesFirstLayerWhenInitialized()
        {
            Layer layer = new();
            image.Layers.Add(layer);

            this.document = new Document(image);

            Assert.That(document.ActiveLayer, Is.EqualTo(layer));
        }

        [Test]
        public void ActivatesNewlyAddedLayer()
        {
            Layer layer1 = new Layer { Name = "layer1" };
            Layer layer2 = new Layer { Name = "layer2" };
            
            image.Layers.Add(layer1);
            image.Layers.Add(layer2);

            Assert.That(document.ActiveLayer, Is.EqualTo(layer2));
        }

        [Test]
        public void ActivatesPreviousLayerWhenActiveLayerIsRemoved()
        {
            Layer layer1 = new Layer { Name = "layer1" };
            Layer layer2 = new Layer { Name = "layer2" };
            Layer layer3 = new Layer { Name = "layer3" };

            image.Layers.Add(layer1);
            image.Layers.Add(layer2);
            image.Layers.Add(layer3);
            image.Layers.Remove(layer3);

            Assert.That(document.ActiveLayer, Is.EqualTo(layer2));
        }

        [Test]
        public void ActivatesNextLayerWhenFirstLayerIsRemoved()
        {
            Layer layer1 = new Layer { Name = "layer1" };
            Layer layer2 = new Layer { Name = "layer2" };

            image.Layers.Add(layer1);
            image.Layers.Add(layer2);
            document.ActiveLayer = layer1;
            image.Layers.Remove(layer1);

            Assert.That(document.ActiveLayer, Is.EqualTo(layer2));
        }

        [Test]
        public void UnsetsActiveLayerWhenAllLayersAreRemoved()
        {
            Layer layer = new();

            image.Layers.Add(layer);
            image.Layers.Remove(layer);

            Assert.That(document.ActiveLayer, Is.Null);
        }

        [Test]
        public void InitializesViewport()
        {
            Assert.That(document.Viewport.ImageSize, Is.EqualTo(image.Size));
        }

        [Test]
        public void UpdatesViewportWhenImageSizeChanges()
        {
            image.Size *= 2;

            Assert.That(document.Viewport.ImageSize, Is.EqualTo(image.Size));
        }

        private static void ProduceChange(Image image)
        {
            image.Size *= 2;
        }
    }
}
