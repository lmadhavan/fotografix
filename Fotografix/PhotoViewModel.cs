using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;

namespace Fotografix
{
    public sealed class PhotoViewModel : NotifyPropertyChangedBase
    {
        private readonly Photo photo;

        public PhotoViewModel(Photo photo)
        {
            this.photo = photo;
            photo.Changed += (s, e) => RaisePropertyChanged("");
        }

        public string Name => photo.Name;
        public NotifyTaskCompletion<ThumbnailViewModel> Thumbnail => new NotifyTaskCompletion<ThumbnailViewModel>(ThumbnailViewModel.CreateAsync(photo));
        public NotifyTaskCompletion<bool> IsEdited => new NotifyTaskCompletion<bool>(photo.HasAdjustmentAsync());

        internal Task<PhotoEditor> CreateEditorAsync(ICanvasResourceCreatorWithDpi canvasResourceCreator)
        {
            return PhotoEditor.CreateAsync(photo, canvasResourceCreator);
        }
    }
}
