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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Abstract base class for view-model classes that need to implement INotifyPropertyChanged.
    /// </summary>
    public abstract class ObservablePropertiesObject : INotifyPropertyChanged
    {
#if DEBUG
        private static int _nextObjectId;

        /// <summary>
        /// Gets a unique identifier for the object, used for debugging purposes.
        /// </summary>
        public int ObjectDebugId { get; } = _nextObjectId++;
#endif //  DEBUG

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event, notifying subscribers of a change in the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed. Automatically inferred if not specified.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for a specific property name.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void RaiseExplicitPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}