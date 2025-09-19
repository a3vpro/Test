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
using System.Runtime.Serialization;
using System.Security;

namespace VisionNet.Core.Exceptions
{
    /// <summary>
    /// Exception that signals a configuration parameter is missing, malformed, or otherwise invalid while configuring VisionNet components.
    /// </summary>
    public class InvalidConfigurationParameterException : VisionNetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationParameterException"/> class for scenarios where a configuration parameter is invalid and no additional context is available.
        /// </summary>
        public InvalidConfigurationParameterException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationParameterException"/> class using the specified error message when a configuration parameter is invalid.
        /// </summary>
        /// <param name="message">The descriptive error message that explains the invalid configuration parameter.</param>
        public InvalidConfigurationParameterException(string message): base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationParameterException"/> class with a specified error message and an inner exception when a configuration parameter is invalid due to an underlying failure.
        /// </summary>
        /// <param name="message">The descriptive error message that explains the invalid configuration parameter.</param>
        /// <param name="innerException">The underlying exception that provides additional detail about the invalid configuration state.</param>
        public InvalidConfigurationParameterException(string message, Exception innerException): base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationParameterException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialized object data about the invalid configuration parameter.</param>
        /// <param name="context">The contextual information about the source or destination of the serialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="info"/> is <see langword="null"/>.</exception>
        /// <exception cref="SerializationException">Thrown when the serialized data is incomplete, corrupted, or missing the required class information.</exception>
        [SecuritySafeCritical]
        protected InvalidConfigurationParameterException(SerializationInfo info, StreamingContext context): base(info, context) { }
    }
}
