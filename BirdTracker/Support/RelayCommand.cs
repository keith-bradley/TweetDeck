﻿using System;
using System.Windows.Input;

/// Author: Keith Bradley
///         Ottawa, Ontario, Canada
///         Copyright 2015

namespace BirdTracker.Support
{
    public class RelayCommand : ICommand
    {
        private Action<object> _action;

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _action(parameter);
            }
            else
            {
                _action("No Parameter");
            }
        }

        #endregion
    }
}
