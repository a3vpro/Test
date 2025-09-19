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
    /// Exposes the ability for an object to proactively release resources and reset transient state so it can be safely reused or disposed without leaving residual data.
    /// </summary>
    public interface ICleanable
    {
        /// <summary>
        /// Executes the cleanup sequence required to return the implementer to a pristine state, releasing unmanaged handles and clearing caches as appropriate for the concrete implementation.
        /// </summary>
        void CleanUp();
    }

}
