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
    /// Represents errors encountered while starting the VisionNet application that prevent the host from completing its startup
    /// sequence.
    /// </summary>
    /// <remarks>
    /// Throw this exception when a required service, configuration value, or dependency fails during application bootstrap in a
    /// way that stops the application from becoming operational.
    /// </remarks>
    public class StartingAppException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingAppException"/> class with a default message describing a
        /// startup failure.
        /// </summary>
        public StartingAppException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartingAppException"/> class with a specific error message that explains
        /// the reason the application startup failed.
        /// </summary>
        /// <param name="message">
        /// A human-readable description of the startup failure. Provide actionable context such as the missing configuration key
        /// or failing dependency.
        /// </param>
        public StartingAppException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartingAppException"/> class with a specified error message and a
        /// reference to the inner exception that caused the startup failure.
        /// </summary>
        /// <param name="message">
        /// A description of the startup error. The message should clarify why the application could not complete initialization.
        /// </param>
        /// <param name="innerException">
        /// The exception that triggered the startup failure. Use this to preserve the original stack trace and error details.
        /// </param>
        public StartingAppException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartingAppException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> instance that stores the serialized exception data, including the startup error
        /// message and any inner exception details.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> structure that describes the source or destination for the serialization operation.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
        /// <exception cref="SerializationException">The class name is <see langword="null"/> or the <see cref="Exception.HResult"/> value is zero.</exception>
        [SecuritySafeCritical]
        protected StartingAppException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
