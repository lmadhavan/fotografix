using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fotografix.Editor
{
    public abstract class AsyncCommand : ICommand
    {
        public abstract event EventHandler CanExecuteChanged;

        public abstract bool CanExecute();
        public abstract Task ExecuteAsync();

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync();
        }
    }
}
