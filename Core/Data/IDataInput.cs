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
    /// Defines an interface for handling input data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input data.</typeparam>
    public interface IDataInput<T>
    {
        /// <summary>
        /// Gets or sets the input data for the instance.
        /// </summary>
        T InputData { get; set; }
    }
}
