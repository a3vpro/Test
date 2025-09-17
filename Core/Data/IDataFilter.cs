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
    /// Defines an interface for filtering data of type <typeparamref name="T"/>.
    /// This interface extends <see cref="IDataInputOutput{T, T}"/> and provides a property to indicate whether the data is accepted by the filter.
    /// </summary>
    /// <typeparam name="T">The type of data to be filtered.</typeparam>
    public interface IDataFilter<T> : IDataInputOutput<T, T>
    {
        /// <summary>
        /// Gets a value indicating whether the data is accepted by the filter.
        /// </summary>
        bool Accepted { get; }
    }

}
