//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using VisionNet.Core.Events;
using System.IO;
using VisionNet.Core.SafeObjects;
using VisionNet.Core.Comparisons;
using VisionNet.Core.Exceptions;
using VisionNet.Core.Types;
using VisionNet.Core.Tasks;

namespace VisionNet.Core.Monitoring
{
    /// <summary>
    /// Represents a monitorized value, which stores a value and tracks changes to it, including timing information.
    /// Provides mechanisms to notify observers when the value changes, as well as exception handling.
    /// </summary>
    public class MonitorizedValue : SafeObject, IMonitorizedValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorizedValue"/> class with default settings.
        /// Resets the chronometer used to track the time since the last update.
        /// </summary>
        public MonitorizedValue()
            : base()
        {
            _resetChronometer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorizedValue"/> class with the specified data type and optional default value.
        /// Resets the chronometer used to track the time since the last update.
        /// </summary>
        /// <param name="dataType">The type of data to be stored in the monitorized value.</param>
        /// <param name="defaultValue">The default value used to initialize the monitorized value. This parameter is optional and can be omitted if no default value is desired.</param>
        /// <param name="preferences">The conversion preferences used when initializing the value.</param>
        public MonitorizedValue(TypeCode dataType, object defaultValue = null, TypeConversionPreferences preferences = TypeConversionPreferences.None)
            : base(dataType, defaultValue, preferences)
        {
            _resetChronometer();
        }

        #region ITimeMonitorized

        private Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Gets the date and time when the value was last updated.
        /// </summary>
        public DateTime LastUpdate { get; private set; } = DateTime.Now;

        /// <summary>
        /// Gets the time duration since the last update.
        /// </summary>
        public TimeSpan LastDuration => _stopwatch.Elapsed;

        /// <summary>
        /// Resets the chronometer, clearing the time elapsed and updating the LastUpdate property.
        /// </summary>
        private void _resetChronometer()
        {
            _stopwatch.Reset();
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Starts or restarts the chronometer and sets the LastUpdate property to the current time.
        /// </summary>
        private void _startChronometer()
        {
            _stopwatch.Reset();
            LastUpdate = DateTime.Now;
            _stopwatch.Start();
        }

        /// <summary>
        /// Stops the chronometer.
        /// </summary>
        private void _stopChronometer()
        {
            _stopwatch.Stop();
        }

        /// <summary>
        /// Stops the chronometer and immediately starts it again.
        /// </summary>
        private void _restartChronometer()
        {
            _stopChronometer();
            _startChronometer();
        }

        #endregion

        #region IObservableValue

        private List<object> _queriers = new List<object>();
        private bool _isChanged;

        /// <summary>
        /// Gets or sets information about the caller that triggered the change.
        /// </summary>
        public CallerInformation CallerInformation { get; private set; }

        /// <summary>
        /// Attempts to set the value of the monitorized value.
        /// If the value is successfully set, returns true; otherwise, returns false.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the value was successfully set; otherwise, false.</returns>
        public override bool TrySetValue(object value)
        {
            return TrySetValue(value, false, null, "");
        }

        /// <summary>
        /// Attempts to set the value of the monitorized value, optionally forcing an update even if the value is the same.
        /// Raises a change notification event if the value is different from the previous one.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="forceUpdate">If true, the value will be set even if it is equal to the current value.</param>
        /// <param name="callerIdentifier">An optional identifier for the caller.</param>
        /// <param name="description">An optional description of the caller.</param>
        /// <returns>True if the value was successfully set; otherwise, false.</returns>
        public bool TrySetValue(object value, bool forceUpdate = false, object callerIdentifier = null, string description = "")
        {
            bool result = false;
            if (Enabled && IsValidValue(value))
            {
                lock (_lockObject)
                {
                    if (forceUpdate || !value.SafeAreEqualTo(_value))
                    {
                        _isChanged = true;
                        _queriers.Clear();
                        CallerInformation = CallerInformationFactory.Create(callerIdentifier, description);
                        _restartChronometer();
                        RaiseValueChanged(this, new ValueChangedEventArgs<object>(_value, value, CallerInformation));
                        _value = value;
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Sets the value of the monitorized value after a specified delay, regardless of whether the value has changed or not.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <param name="delayMs">The delay, in milliseconds, before setting the value.</param>
        /// <param name="forceUpdate">If true, the value will be set even if it is equal to the current value.</param>
        /// <param name="callerIdentifier">An optional identifier for the caller.</param>
        /// <param name="description">An optional description of the caller.</param>
        /// <returns>True if the value is successfully scheduled for update; otherwise, false.</returns>
        public bool TrySetValueDelayed(object value, int delayMs, bool forceUpdate = false, object callerIdentifier = null, string description = "")
        {
            DelayedTask.StartNew(_ => TrySetValue(value), delayMs);
            return true;
        }

        /// <summary>
        /// Checks if the monitorized value has changed since the last query.
        /// If an object has not been queried before, it will always return true.
        /// </summary>
        /// <param name="queryIdentifier">A unique identifier for the query.</param>
        /// <returns>True if the value has changed since the last query; otherwise, false.</returns>
        public bool IsChanged(object queryIdentifier)
        {
            bool changed = _isChanged;
            lock (_lockObject)
            {
                if (_isChanged)
                {
                    changed = !_queriers.Contains(queryIdentifier);
                    if (changed)
                        _queriers.Add(queryIdentifier);
                }
            }
            return changed;
        }

        /// <summary>
        /// Raises the ValueChanged event to notify subscribers that the value has changed.
        /// Catches any exceptions thrown by event handlers and notifies via the exception handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The event arguments containing the old and new values.</param>
        protected void RaiseValueChanged(object sender, ValueChangedEventArgs<object> eventArgs)
        {
            try
            {
                ValueChanged?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                RaiseExceptionNotification(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Occurs when the value of the monitorized value changes.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<object>> ValueChanged;

        #endregion

        #region IExceptionObservable

        /// <summary>
        /// Raises the ExceptionRaised event to notify subscribers of an exception that occurred.
        /// Catches any exceptions thrown by event handlers and logs them to the console.
        /// </summary>
        /// <param name="sender">The source of the exception.</param>
        /// <param name="eventArgs">The event arguments containing the exception details.</param>
        protected void RaiseExceptionNotification(object sender, ErrorEventArgs eventArgs)
        {
            try
            {
                ExceptionRaised?.Invoke(sender, eventArgs);
            }
            catch (Exception ex)
            {
                ex.LogToConsole(nameof(RaiseExceptionNotification));
            }
        }

        /// <summary>
        /// Occurs when an exception is raised by the monitorized value.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ExceptionRaised;

        #endregion

        /// <summary>
        /// Gets or sets the index of the monitorized value.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets the name of the monitorized value.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the monitorized value.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the monitorized value is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }

}
