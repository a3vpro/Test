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
namespace VisionNet.Core.SafeObjects
{
    /// <summary>
    /// Extends <see cref="IReadonlySafeObject{TType}"/> to provide functionality for safely modifying the value of an object of type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type of the underlying data object.</typeparam>
    public interface ISafeObject<TType> : IReadonlySafeObject<TType>
    {
        /// <summary>
        /// Attempts to set the value of the safe object to the provided value, ensuring type safety and validation.
        /// </summary>
        /// <param name="value">The value to be set, which must be compatible with <typeparamref name="TType"/>.</param>
        /// <returns>True if the value was successfully set and is valid; otherwise, false.</returns>
        /// <remarks>
        /// This method ensures that the value conforms to the constraints and validation rules defined for <typeparamref name="TType"/>.
        /// If the value is not valid or the operation fails for any reason, the method returns false and the state of the safe object remains unchanged.
        /// </remarks>
        bool TrySetValue(object value);

        /// <summary>
        /// Get or set the value of the safe object to the provided value, ensuring type safety and validation.
        /// </summary>
        new object Value { get; set; }

        /// <summary>
        /// Clears the instance value by replacing it with the default value.
        /// </summary>
        void Clear();
    }

}
