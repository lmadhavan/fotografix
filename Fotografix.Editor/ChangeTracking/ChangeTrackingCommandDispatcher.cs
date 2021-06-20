using Fotografix.Editor.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Fotografix.Editor.ChangeTracking
{
    public sealed class ChangeTrackingCommandDispatcher : ICommandDispatcher, IHistory, IDisposable
    {
        private readonly ICommandDispatcher dispatcher;
        private readonly IAppendableHistory history;
        private readonly Image image;

        private List<IChange> changeGroup;
        private bool ignoreChanges;

        public ChangeTrackingCommandDispatcher(Image image, ICommandDispatcher dispatcher) : this(image, dispatcher, new History())
        {
        }

        public ChangeTrackingCommandDispatcher(Image image, ICommandDispatcher dispatcher, IAppendableHistory history)
        {
            this.image = image;
            image.ContentChanged += Image_ContentChanged;

            this.dispatcher = dispatcher;
            this.history = history;
        }

        public void Dispose()
        {
            image.ContentChanged -= Image_ContentChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => history.PropertyChanged += value;
            remove => history.PropertyChanged -= value;
        }

        public bool CanUndo => history.CanUndo;
        public bool CanRedo => history.CanRedo;

        public async Task DispatchAsync<T>(T command)
        {
            try
            {
                this.changeGroup = new List<IChange>();
                await dispatcher.DispatchAsync(command);
            }
            finally
            {
                if (changeGroup.Count > 0)
                {
                    history.Add(new CompositeChange(changeGroup));
                    image.SetDirty(true);
                }

                this.changeGroup = null;
            }
        }

        public void Undo()
        {
            try
            {
                this.ignoreChanges = true;
                history.Undo();
                image.SetDirty(true);
            }
            finally
            {
                this.ignoreChanges = false;
            }
        }

        public void Redo()
        {
            try
            {
                this.ignoreChanges = true;
                history.Redo();
                image.SetDirty(true);
            }
            finally
            {
                this.ignoreChanges = false;
            }
        }

        private void Image_ContentChanged(object sender, ContentChangedEventArgs e)
        {
            if (e.Change != null && !ignoreChanges)
            {
                if (changeGroup != null)
                {
                    changeGroup.Add(e.Change);
                }
                else
                {
                    history.Add(e.Change);
                    image.SetDirty(true);
                }
            }
        }
    }
}
