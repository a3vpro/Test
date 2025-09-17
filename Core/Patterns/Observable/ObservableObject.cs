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

using System.ComponentModel;

namespace VisionNet.Core.Patterns
{
    public class ObservableObject : INotifyPropertyChanged, IObservableObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> The RaisePropertyChanged function is a helper function that raises the PropertyChanged event for the specified property.</summary>
        /// <param name="string propName"> The name of the property that is being changed.</param>
        /// <returns> Nothing.</returns>
        protected virtual void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
