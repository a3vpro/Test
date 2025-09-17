//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 28-08-2021
// Description      : v1.0.0
//
// Copyright        : (C)  2021 by Sothis. All rights reserved.       
//----------------------------------------------------------------------------

using System;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Represents an <see cref="IRelayCommand"/> implementation that delegates command logic to callbacks.
    /// </summary>
    public class RelayCommand : IRelayCommand
    {
        #region Fields

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        #endregion Fields

        #region Constructors

        
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class with the specified execution delegate.
        /// </summary>
        /// <param name="execute">The delegate to invoke when <see cref="Execute(object)"/> is called; must not be <see langword="null"/>.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class with the specified execution and availability delegates.
        /// </summary>
        /// <param name="execute">The delegate to invoke when <see cref="Execute(object)"/> is called; must not be <see langword="null"/>.</param>
        /// <param name="canExecute">An optional delegate that determines whether the command can execute. If <see langword="null"/>, the command is always enabled.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        
        /// <summary>
        /// Determines whether the command can execute by invoking the availability delegate if one was supplied.
        /// </summary>
        /// <param name="parameter">Command parameter supplied by the caller. This implementation ignores the value.</param>
        /// <returns><see langword="true"/> if the command can execute or no availability delegate was provided; otherwise, <see langword="false"/>.</returns>
        public bool CanExecute(object parameter)
        {
            return null == _canExecute ? true : _canExecute();
        }


        /// <summary>
        /// Executes the command by invoking the execution delegate supplied to the constructor.
        /// </summary>
        /// <param name="parameter">Command parameter supplied by the caller. This implementation ignores the value.</param>
        public void Execute(object parameter)
        {
            _execute();
        }


        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event to notify listeners that the return value of <see cref="CanExecute(object)"/> might have changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (null != this.CanExecuteChanged)
                this.CanExecuteChanged.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when changes affecting whether the command should execute are detected. Raised by <see cref="RaiseCanExecuteChanged"/>.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion ICommand Members
    }
}
