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

// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections;

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Arguments to the ItemsAdded and ItemsRemoved events.
    /// </summary>
    public class CollectionItemsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The collection of items that changed.
        /// </summary>
        private readonly ICollection _items;

        
        /// <summary> The CollectionItemsChangedEventArgs function is a constructor that takes an ICollection as its only parameter. It then sets the _items variable to the value of items.</summary>
        /// <param name="items"> The collection of items that were added or removed.</param>
        /// <returns> The items.</returns>
        public CollectionItemsChangedEventArgs(ICollection items)
        {
            _items = items;
        }

        /// <summary>
        /// The collection of items that changed.
        /// </summary>
        public ICollection Items => _items;
    }
}
