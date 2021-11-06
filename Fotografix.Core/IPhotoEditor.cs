using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Fotografix
{
    public interface IPhotoEditor : INotifyPropertyChanged, IDisposable
    {
        IPhotoAdjustment Adjustment { get; }
        bool AdjustmentEnabled { get; set; }
        void Draw(CanvasDrawingSession ds);
        void ResetAdjustment();

        float RenderScale { get; set; }
        Size RenderSize { get; }
        void SetRenderSize(Size size);

        Task SaveAsync();
        Task<StorageFile> ExportAsync(StorageFolder folder);

        event EventHandler Invalidated;
    }
}