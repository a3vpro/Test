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
using VisionNet.Core.Abstractions;

namespace VisionNet.Core.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when a check operation fails in the VisionNet framework.
    /// </summary>
    public class CheckException : VisionNetException
    {
        /// <summary>
        /// Gets or sets the invalid check result that caused the exception.
        /// </summary>
        public InvalidCheckResult InvalidCheckResult { get; protected set; } = new InvalidCheckResult();

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckException"/> class.
        /// </summary>
        public CheckException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckException"/> class with the specified invalid check result.
        /// </summary>
        /// <param name="invalidCheckResult">The result of the invalid check that triggered the exception.</param>
        public CheckException(InvalidCheckResult invalidCheckResult) : base(invalidCheckResult.ToString())
        {
            InvalidCheckResult = invalidCheckResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckException"/> class with the specified invalid check result 
        /// and a reference to the inner exception that caused this exception.
        /// </summary>
        /// <param name="invalidCheckResult">The result of the invalid check that triggered the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public CheckException(InvalidCheckResult invalidCheckResult, Exception innerException) : base(invalidCheckResult.ToString(), innerException)
        {
            InvalidCheckResult = invalidCheckResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized data of the exception.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that provides contextual information about the source or destination of the serialization.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="info"/> parameter is null.</exception>
        /// <exception cref="SerializationException">Thrown when the class name is null or the <see cref="System.Exception.HResult"/> is zero (0).</exception>
        [SecuritySafeCritical]
        protected CheckException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
