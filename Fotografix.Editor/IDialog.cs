using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public interface IDialog<T>
    {
        Task<bool> ShowAsync(T parameters);
    }
}
