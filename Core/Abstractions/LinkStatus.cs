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
namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// State of the link
    /// </summary>
    public enum LinkStatus
    {
        /// <summary>
        /// The objects are unlinked
        /// </summary>
        Unlinked,
        /// <summary>
        /// The objects are linked
        /// </summary>
        Linked
    }
}
