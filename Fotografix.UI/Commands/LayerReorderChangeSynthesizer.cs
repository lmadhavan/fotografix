using Fotografix.Editor.Commands;
using System;
using System.Collections.Specialized;

namespace Fotografix.UI.Commands
{
    /// <summary>
    /// Synthesizes a <see cref="ReorderLayerCommand"/> change when a reordering is detected in the layer list.
    /// </summary>
    /// <remarks>
    /// List reordering is performed directly by the XAML framework by issuing a pair of RemoveAt-Insert calls
    /// to the list. This class detects such reorderings and synthesizes a <see cref="ReorderLayerCommand"/>
    /// change.
    /// </remarks>
    public sealed class LayerReorderChangeSynthesizer : IDisposable
    {
        private readonly Image image;
        private readonly CommandService commandService;

        private Layer fromLayer;
        private int fromIndex;

        public LayerReorderChangeSynthesizer(Image image, CommandService commandService)
        {
            this.image = image;
            image.Layers.CollectionChanged += OnLayerCollectionChanged;

            this.commandService = commandService;
        }

        public void Dispose()
        {
            image.Layers.CollectionChanged -= OnLayerCollectionChanged;
        }

        public event ChangeSynthesizedEventHandler ChangeSynthesized;

        private void OnLayerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.fromLayer = (Layer)e.OldItems[0];
                this.fromIndex = e.OldStartingIndex;
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Layer toLayer = (Layer)e.NewItems[0];

                if (fromLayer == toLayer)
                {
                    int toIndex = e.NewStartingIndex;
                    TrySynthesizeChange(fromIndex, toIndex);
                }
                else
                {
                    this.fromLayer = null;
                }
            }
        }

        private void TrySynthesizeChange(int fromIndex, int toIndex)
        {
            if (!commandService.IsBusy)
            {
                IChange change = new ReorderLayerCommand(image, fromIndex, toIndex);
                commandService.AddChange(change);
                ChangeSynthesized?.Invoke(this, new ChangeSynthesizedEventArgs(change));
            }
        }
    }

    public delegate void ChangeSynthesizedEventHandler(object sender, ChangeSynthesizedEventArgs args);

    public sealed class ChangeSynthesizedEventArgs : EventArgs
    {
        public ChangeSynthesizedEventArgs(IChange change)
        {
            this.Change = change;
        }

        public IChange Change { get; }
    }
}
