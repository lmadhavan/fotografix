using System.ComponentModel;

namespace Fotografix.Editor.ChangeTracking
{
    public interface IHistory : INotifyPropertyChanged
    {
        bool CanUndo { get; }
        bool CanRedo { get; }

        void Undo();
        void Redo();
    }
}
