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
    /// Specifies a ableable and disableable instance
    /// </summary>
    public interface IDisableable
    {
        /// <summary>
        /// Specifies that the instance is enabled or disabled
        /// </summary>
        bool Enabled { get; set; }
    }
}
