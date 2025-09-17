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
    public class RelayCommand : IRelayCommand
    {
        #region Fields

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        #endregion Fields

        #region Constructors

        
        /// <summary> The RelayCommand function is a constructor that takes in an Action and Func.
        /// The Action is the command to be executed, while the Func determines whether or not
        /// the command can be executed.</summary>
        /// <param name="Action execute"> The action to be executed.</param>
        /// <returns> A new relaycommand object</returns>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        
        /// <summary> The RelayCommand function is a constructor that takes two parameters: an Action and a Func.
        /// The Action parameter is the method to be executed when the command is invoked. 
        /// The Func parameter returns true if this command can be executed; otherwise, false.</summary>
        /// <param name="Action execute"> </param>
        /// <param name="Func&lt;bool&gt; canExecute"> /// the canexecute parameter is a function that returns true or false. 
        /// it's used to determine whether the command should be executed or not. 
        /// if it returns true, the command will be executed, otherwise it won't.</param>
        /// <returns> A relaycommand object</returns>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        
        /// <summary> The CanExecute function is used to determine whether or not the command can be executed.
        /// If no CanExecute function was provided when the command was created, this will always return true.</summary>
        /// <param name="object parameter"> ///     the parameter is not used in this example.
        /// </param>
        /// <returns> A boolean value. if the function returns true, the button is enabled and clickable. if it returns false, the button is disabled.</returns>
        public bool CanExecute(object parameter)
        {
            return null == _canExecute ? true : _canExecute();
        }

        
        /// <summary> The Execute function is called when the command is executed.
        /// It calls the _execute function, which was passed in as a parameter to 
        /// the constructor.</summary>
        /// <param name="object parameter"> /// the parameter is used to pass a value to the command. 
        /// this can be useful if you want to pass an object or some other data type. 
        /// </param>
        /// <returns> A boolean value</returns>
        public void Execute(object parameter)
        {
            _execute();
        }

        
        /// <summary> The RaiseCanExecuteChanged function is used to notify the UI that the CanExecute function has changed.
        /// This allows for a more responsive UI.</summary>
        /// <returns> Void.</returns>
        public void RaiseCanExecuteChanged()
        {
            if (null != this.CanExecuteChanged)
                this.CanExecuteChanged.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;

        #endregion ICommand Members
    }
}
