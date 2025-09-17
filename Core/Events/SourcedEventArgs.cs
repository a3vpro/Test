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
using VisionNet.Core.Patterns;

namespace VisionNet.Core.Events
{
    /// <summary>
    /// A generic EventArgs instance that uses generics for both the event arguments and an index value.
    /// </summary>
    /// <typeparam name="S">The type of the index key.</typeparam>
    /// <typeparam name="T">The type of the event argument's value.</typeparam>
    public class SourcedEventArgs<S, T> : EventArgs<T>, IEntity<S>
    {
        /// <summary>
        /// Gets the index key associated with the event arguments.
        /// </summary>
        public S Index { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SourcedEventArgs{S, T}"/> class with the specified index and value.
        /// </summary>
        /// <param name="index">The index key associated with the event.</param>
        /// <param name="value">The value of the event argument.</param>
        public SourcedEventArgs(S index, T value)
            : base(value)
        {
            Index = index;
        }
    }
}
