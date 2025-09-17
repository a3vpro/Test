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

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Defines an interface for objects that can be copied or cloned.
    /// </summary>
    public interface ICopiable : ICloneable
    {
        /// <summary>
        /// Clones the current object and copies its data to the specified destination object.
        /// </summary>
        /// <param name="destiny">The object that will receive the copied data.</param>
        void CloneTo(ref object destiny);
    }

}
