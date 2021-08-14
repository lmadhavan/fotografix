using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fotografix.Editor
{
    public abstract class AsyncCommand : ICommand
    {
        public abstract event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);
        public abstract Task ExecuteAsync(object parameter);

        public bool CanExecute()
        {
            return CanExecute(null);
        }

        public Task ExecuteAsync()
        {
            return ExecuteAsync(null);
        }

        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
    }
}
