using Fotografix.Editor.Commands;
using Moq;
using NUnit.Framework;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Editor.ChangeTracking
{
    [TestFixture]
    public class ChangeTrackingCommandDispatcherTest
    {
        private Image image;
        private FakeCommandDispatcher baseDispatcher;
        private Mock<IAppendableHistory> history;
        private ChangeTrackingCommandDispatcher changeTrackingDispatcher;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(new Size(10, 10));
            this.baseDispatcher = new FakeCommandDispatcher(image);
            this.history = new Mock<IAppendableHistory>();
            this.changeTrackingDispatcher = new ChangeTrackingCommandDispatcher(image, baseDispatcher, history.Object);
        }

        [Test]
        public void TracksChangesToImage()
        {
            ProduceChange(image);

            history.Verify(h => h.Add(It.IsAny<IChange>()));
            Assert.IsTrue(image.IsDirty(), "Dirty");
        }

        [Test]
        public async Task GroupsChangesProducedByCommand()
        {
            await changeTrackingDispatcher.DispatchAsync(3);

            history.Verify(h => h.Add(It.IsAny<CompositeChange>()), Times.Once);
            Assert.IsTrue(image.IsDirty(), "Dirty");
        }

        [Test]
        public async Task DoesNotAddEmptyChangeGroupsToHistory()
        {
            await changeTrackingDispatcher.DispatchAsync(0);
            
            history.Verify(h => h.Add(It.IsAny<IChange>()), Times.Never);
            Assert.IsFalse(image.IsDirty(), "Dirty");
        }

        [Test]
        public void UndoingMarksImageDirty()
        {
            changeTrackingDispatcher.Undo();

            history.Verify(h => h.Undo());
            Assert.IsTrue(image.IsDirty(), "Dirty");
        }

        [Test]
        public void IgnoresAdditionalChangesWhenUndoing()
        {
            history.Setup(h => h.Undo()).Callback(() => ProduceChange(image));

            changeTrackingDispatcher.Undo();

            history.Verify(h => h.Add(It.IsAny<IChange>()), Times.Never);
        }

        [Test]
        public void RedoingMarksImageDirty()
        {
            changeTrackingDispatcher.Redo();

            history.Verify(h => h.Redo());
            Assert.IsTrue(image.IsDirty(), "Dirty");
        }

        [Test]
        public void IgnoresAdditionalChangesWhenRedoing()
        {
            history.Setup(h => h.Redo()).Callback(() => ProduceChange(image));

            changeTrackingDispatcher.Redo();

            history.Verify(h => h.Add(It.IsAny<IChange>()), Times.Never);
        }

        private static void ProduceChange(Image image)
        {
            image.Size *= 2;
        }

        private sealed class FakeCommandDispatcher : ICommandDispatcher
        {
            private readonly Image image;

            public FakeCommandDispatcher(Image image)
            {
                this.image = image;
            }

            public Task DispatchAsync<T>(T command)
            {
                int changes = (int)(object)command;

                for (int i = 0; i < changes; i++)
                {
                    ProduceChange(image);
                }

                return Task.CompletedTask;
            }
        }
    }
}
