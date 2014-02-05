// Copyright (c) 2013 Ronald Valkenburg
// This software is licensed under the MIT License (see LICENSE file for details)

using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NDragDrop.TestApplication
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<object> _canExecute;
        private List<WeakReference> canExecuteChangedHandlers;
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<T> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }


        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
