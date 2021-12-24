using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fotografix
{
    public sealed class DelegateCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Func<Task> execute;
        private bool executing;

        public DelegateCommand(Action execute) : this(() => true, () => { execute(); return Task.CompletedTask; })
        {
        }

        public DelegateCommand(Func<Task> execute) : this(() => true, execute)
        {
        }

        public DelegateCommand(Func<bool> canExecute, Func<Task> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public async void Execute(object parameter)
        {
            if (executing)
            {
                /*
                 * This scenario can occur in two situations:
                 * 1. The user invokes the command again before the previous async execution has completed.
                 * 2. A bug in the XAML framework that sometimes triggers a command twice when using a keyboard accelerator.
                 */
                Debug.WriteLine("Skipping command execution since previous execution has not completed yet");
                return;
            }

            try
            {
                this.executing = true;
                await execute();
            }
            finally
            {
                this.executing = false;
            }
        }
    }
}
