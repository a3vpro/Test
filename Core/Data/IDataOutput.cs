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
namespace VisionNet.Core.Data
{
    /// <summary>
    /// Defines an interface for handling output data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the output data.</typeparam>
    public interface IDataOutput<T>
    {
        /// <summary>
        /// Gets the data produced by the instance.
        /// </summary>
        T OutputData { get; }
    }

}
