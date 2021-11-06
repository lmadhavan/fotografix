using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Fotografix
{
    public sealed class FakePhotoEditor : NotifyPropertyChangedBase, IPhotoEditor
    {
        private float renderScale = 1;
        private Size renderSize;

        public IPhotoAdjustment Adjustment => throw new NotImplementedException();
        public bool AdjustmentEnabled { get; set; } = true;

        public float RenderScale
        {
            get => renderScale;
            
            set
            {
                this.renderScale = value;
                this.RenderSize = default;
            }
        }

        public Size RenderSize
        {
            get => renderSize;
            set => SetProperty(ref renderSize, value);
        }

        public void SetRenderSize(Size size)
        {
            this.RenderSize = size;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Draw(CanvasDrawingSession ds)
        {
            throw new NotImplementedException();
        }

        public Task<StorageFile> ExportAsync(StorageFolder folder)
        {
            throw new NotImplementedException();
        }

        public void ResetAdjustment()
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Invalidated;
    }
}
