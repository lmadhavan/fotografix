using Fotografix.Editor.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class Document : NotifyPropertyChangedBase, IDisposable, IHistory
    {
        private readonly Image image;
        private readonly IAppendableHistory history;

        private List<IChange> changeGroup;
        private bool ignoreChanges;

        public Document(Image image) : this(image, new History())
        {
        }

        public Document(Image image, IAppendableHistory history)
        {
            this.image = image;
            image.ContentChanged += Image_ContentChanged;

            this.history = history;
            history.PropertyChanged += History_PropertyChanged;
        }

        public void Dispose()
        {
            history.PropertyChanged -= History_PropertyChanged;
            image.ContentChanged -= Image_ContentChanged;
        }

        public Image Image => image;

        public bool CanUndo => history.CanUndo;
        public bool CanRedo => history.CanRedo;

        public IDisposable BeginChangeGroup()
        {
            this.changeGroup = new List<IChange>();
            return new DisposableAction(CommitChangeGroup);
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

        private void CommitChangeGroup()
        {
            if (changeGroup.Count > 0)
            {
                history.Add(new CompositeChange(changeGroup));
                image.SetDirty(true);
            }

            this.changeGroup = null;
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

        private void History_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }
}
