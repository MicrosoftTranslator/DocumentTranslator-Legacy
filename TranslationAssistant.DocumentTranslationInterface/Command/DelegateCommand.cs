// // ----------------------------------------------------------------------
// // <copyright file="DelegateCommand.cs" company="Microsoft Corporation">
// // Copyright (c) Microsoft Corporation.
// // All rights reserved.
// // THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// // KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// // IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// // PARTICULAR PURPOSE.
// // </copyright>
// // ----------------------------------------------------------------------
// // <summary>DelegateCommand.cs</summary>
// // ----------------------------------------------------------------------

namespace TranslationAssistant.DocumentTranslationInterface.Command
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    #endregion

    /// <summary>
    ///     The delegate command.
    /// </summary>
    internal class DelegateCommand : ICommand
    {
        #region Fields

        /// <summary>
        ///     The _can execute method.
        /// </summary>
        private readonly Func<bool> _canExecuteMethod;

        /// <summary>
        ///     The _execute method.
        /// </summary>
        private readonly Action _executeMethod;

        /// <summary>
        ///     The _can execute changed handlers.
        /// </summary>
        private List<WeakReference> _canExecuteChangedHandlers;

        /// <summary>
        ///     The _is automatic requery disabled.
        /// </summary>
        private bool _isAutomaticRequeryDisabled;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand" /> class.
        ///     Constructor
        /// </summary>
        /// <param name="executeMethod">
        ///     The execute Method.
        /// </param>
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand" /> class.
        ///     Constructor
        /// </summary>
        /// <param name="executeMethod">
        ///     The execute Method.
        /// </param>
        /// <param name="canExecuteMethod">
        ///     The can Execute Method.
        /// </param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand" /> class.
        ///     Constructor
        /// </summary>
        /// <param name="executeMethod">
        ///     The execute Method.
        /// </param>
        /// <param name="canExecuteMethod">
        ///     The can Execute Method.
        /// </param>
        /// <param name="isAutomaticRequeryDisabled">
        ///     The is Automatic Requery Disabled.
        /// </param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }

            this._executeMethod = executeMethod;
            this._canExecuteMethod = canExecuteMethod;
            this._isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     ICommand.CanExecuteChanged implementation
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!this._isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }

                CommandManagerHelper.AddWeakReferenceHandler(ref this._canExecuteChangedHandlers, value, 2);
            }

            remove
            {
                if (!this._isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }

                CommandManagerHelper.RemoveWeakReferenceHandler(this._canExecuteChangedHandlers, value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Property to enable or disable CommandManager's automatic requery on this command
        /// </summary>
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return this._isAutomaticRequeryDisabled;
            }

            set
            {
                if (this._isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(this._canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(this._canExecuteChangedHandlers);
                    }

                    this._isAutomaticRequeryDisabled = value;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Method to determine if the command can be executed
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CanExecute()
        {
            if (this._canExecuteMethod != null)
            {
                return this._canExecuteMethod();
            }

            return true;
        }

        /// <summary>
        ///     Execution of the command
        /// </summary>
        public void Execute()
        {
            if (this._executeMethod != null)
            {
                this._executeMethod();
            }
        }

        /// <summary>
        ///     Raises the CanExecuteChaged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     The can execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute();
        }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(this._canExecuteChangedHandlers);
        }

        #endregion
    }

    /// <summary>
    ///     This class allows delegating the commanding logic to methods passed as parameters,
    ///     and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the parameter passed to the delegates
    /// </typeparam>
    public class DelegateCommand<T> : ICommand
    {
        #region Fields

        /// <summary>
        ///     The _can execute method.
        /// </summary>
        private readonly Func<T, bool> _canExecuteMethod;

        /// <summary>
        ///     The _execute method.
        /// </summary>
        private readonly Action<T> _executeMethod;

        /// <summary>
        ///     The _can execute changed handlers.
        /// </summary>
        private List<WeakReference> _canExecuteChangedHandlers;

        /// <summary>
        ///     The _is automatic requery disabled.
        /// </summary>
        private bool _isAutomaticRequeryDisabled;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand{T}" /> class.
        ///     Constructor
        /// </summary>
        /// <param name="executeMethod">
        ///     The execute Method.
        /// </param>
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand{T}" /> class.
        ///     Constructor
        /// </summary>
        /// <param name="executeMethod">
        ///     The execute Method.
        /// </param>
        /// <param name="canExecuteMethod">
        ///     The can Execute Method.
        /// </param>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelegateCommand{T}" /> class.
        ///     Constructor
        /// </summary>
        /// <param name="executeMethod">
        ///     The execute Method.
        /// </param>
        /// <param name="canExecuteMethod">
        ///     The can Execute Method.
        /// </param>
        /// <param name="isAutomaticRequeryDisabled">
        ///     The is Automatic Requery Disabled.
        /// </param>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }

            this._executeMethod = executeMethod;
            this._canExecuteMethod = canExecuteMethod;
            this._isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     ICommand.CanExecuteChanged implementation
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!this._isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }

                CommandManagerHelper.AddWeakReferenceHandler(ref this._canExecuteChangedHandlers, value, 2);
            }

            remove
            {
                if (!this._isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }

                CommandManagerHelper.RemoveWeakReferenceHandler(this._canExecuteChangedHandlers, value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Property to enable or disable CommandManager's automatic requery on this command
        /// </summary>
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return this._isAutomaticRequeryDisabled;
            }

            set
            {
                if (this._isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(this._canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(this._canExecuteChangedHandlers);
                    }

                    this._isAutomaticRequeryDisabled = value;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Method to determine if the command can be executed
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CanExecute(T parameter)
        {
            if (this._canExecuteMethod != null)
            {
                return this._canExecuteMethod(parameter);
            }

            return true;
        }

        /// <summary>
        ///     Execution of the command
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        public void Execute(T parameter)
        {
            if (this._executeMethod != null)
            {
                this._executeMethod(parameter);
            }
        }

        /// <summary>
        ///     Raises the CanExecuteChaged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     The can execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            // if T is of value type and the parameter is not
            // set yet, then return false if CanExecute delegate
            // exists, else return true
            if (parameter == null && typeof(T).IsValueType)
            {
                return this._canExecuteMethod == null;
            }

            return this.CanExecute((T)parameter);
        }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        void ICommand.Execute(object parameter)
        {
            this.Execute((T)parameter);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(this._canExecuteChangedHandlers);
        }

        #endregion
    }

    /// <summary>
    ///     This class contains methods for the CommandManager that help avoid memory leaks by
    ///     using weak references.
    /// </summary>
    internal class CommandManagerHelper
    {
        #region Methods

        /// <summary>
        ///     The add handlers to requery suggested.
        /// </summary>
        /// <param name="handlers">
        ///     The handlers.
        /// </param>
        internal static void AddHandlersToRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    var handler = handlerRef.Target as EventHandler;
                    if (handler != null)
                    {
                        CommandManager.RequerySuggested += handler;
                    }
                }
            }
        }

        /// <summary>
        ///     The add weak reference handler.
        /// </summary>
        /// <param name="handlers">
        ///     The handlers.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler)
        {
            AddWeakReferenceHandler(ref handlers, handler, -1);
        }

        /// <summary>
        ///     The add weak reference handler.
        /// </summary>
        /// <param name="handlers">
        ///     The handlers.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        /// <param name="defaultListSize">
        ///     The default list size.
        /// </param>
        internal static void AddWeakReferenceHandler(
            ref List<WeakReference> handlers,
            EventHandler handler,
            int defaultListSize)
        {
            if (handlers == null)
            {
                handlers = defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>();
            }

            handlers.Add(new WeakReference(handler));
        }

        /// <summary>
        ///     The call weak reference handlers.
        /// </summary>
        /// <param name="handlers">
        ///     The handlers.
        /// </param>
        internal static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                // Take a snapshot of the handlers before we call out to them since the handlers
                // could cause the array to me modified while we are reading it.
                var callees = new EventHandler[handlers.Count];
                int count = 0;

                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    var handler = reference.Target as EventHandler;
                    if (handler == null)
                    {
                        // Clean up old handlers that have been collected
                        handlers.RemoveAt(i);
                    }
                    else
                    {
                        callees[count] = handler;
                        count++;
                    }
                }

                // Call the handlers that we snapshotted
                for (int i = 0; i < count; i++)
                {
                    EventHandler handler = callees[i];
                    handler(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     The remove handlers from requery suggested.
        /// </summary>
        /// <param name="handlers">
        ///     The handlers.
        /// </param>
        internal static void RemoveHandlersFromRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    var handler = handlerRef.Target as EventHandler;
                    if (handler != null)
                    {
                        CommandManager.RequerySuggested -= handler;
                    }
                }
            }
        }

        /// <summary>
        ///     The remove weak reference handler.
        /// </summary>
        /// <param name="handlers">
        ///     The handlers.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        internal static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers != null)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    var existingHandler = reference.Target as EventHandler;
                    if ((existingHandler == null) || (existingHandler == handler))
                    {
                        // Clean up old handlers that have been collected
                        // in addition to the handler that is to be removed.
                        handlers.RemoveAt(i);
                    }
                }
            }
        }

        #endregion
    }
}