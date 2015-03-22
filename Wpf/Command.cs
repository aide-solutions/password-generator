namespace Wpf
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    public sealed class Command<T> : ICommand
    {
        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;

        public Command(Action<T> execute)
            : this(execute, null)
        {
        }

        public Command(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(T parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return parameter == null
                ? true
                : CanExecute((T)parameter);
        }
        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
           Execute((T)parameter);
        }
        [DebuggerStepThrough]
        public void Execute(T parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}