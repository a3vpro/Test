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
    /// State of the reproduction
    /// </summary>
    public enum ReproductionStatus
    {
        /// <summary>
        /// The reproduction is stopped
        /// </summary>
        Stopped,
        /// <summary>
        /// The reproduction is playing
        /// </summary>
        Playing
    }
}
