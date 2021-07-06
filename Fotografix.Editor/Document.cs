using Fotografix.Editor.ChangeTracking;
using Fotografix.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Fotografix.Editor
{
    public sealed class Document : NotifyPropertyChangedBase, IDisposable, IHistory
    {
        private readonly Image image;
        private readonly IAppendableHistory history;

        private IFile file;
        private bool dirty;

        private List<IChange> changeGroup;
        private bool ignoreChanges;

        public Document() : this(new Image())
        {
        }

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

        public IFile File
        {
            get => file;
            set => SetProperty(ref file, value);
        }

        public bool IsDirty
        {
            get => dirty;
            set => SetProperty(ref dirty, value);
        }

        public event EventHandler ContentChanged;

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
                this.IsDirty = true;
            }
            finally
            {
                this.ignoreChanges = false;
                RaiseContentChanged();
            }
        }

        public void Redo()
        {
            try
            {
                this.ignoreChanges = true;
                history.Redo();
                this.IsDirty = true;
            }
            finally
            {
                this.ignoreChanges = false;
                RaiseContentChanged();
            }
        }

        private void CommitChangeGroup()
        {
            if (changeGroup.Count > 0)
            {
                AddChange(new CompositeChange(changeGroup));
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
                    AddChange(e.Change);
                }
            }
        }

        private void History_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void AddChange(IChange change)
        {
            history.Add(change);
            this.IsDirty = true;
            RaiseContentChanged();
        }

        private void RaiseContentChanged()
        {
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
