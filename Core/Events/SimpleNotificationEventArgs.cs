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

namespace VisionNet.Core.Events
{
    /// <summary>
    /// A generical EventArgs instance using generics
    /// </summary>
    /// <typeparam name="T">The type of arguments</typeparam>
    public class SimpleNotificationEventArgs<T>: EventArgs<T>
    {
        /// <summary>
        /// Create a generic EventArgs with the specific value
        /// </summary>
        /// <param name="index">The index key</param>
        public SimpleNotificationEventArgs(T index)
            :base(index)
        {
        }
    }
}
