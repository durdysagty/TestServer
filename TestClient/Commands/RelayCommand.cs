using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestClient.Commands
{
    class RelayCommand : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<object, bool> canExecute;
        public RelayCommand(Func<Task> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public async void Execute(object parameter)
        {
            await execute();
        }
    }
}
