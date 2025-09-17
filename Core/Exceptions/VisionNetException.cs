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
using System.Security.Permissions;

namespace VisionNet.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur while performing VisionNet operations.
    /// </summary>
    [Serializable]
    public class VisionNetException : Exception
    {
        /// <summary>
        /// Gets or sets the resource reference identifier associated with the exception instance.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> token that identifies the resource tied to the failure; this value can be <see langword="null"/>.
        /// </value>
        public string ResourceReferenceProperty { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionNetException"/> class with a default error description.
        /// </summary>
        public VisionNetException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionNetException"/> class with a specific error message.
        /// </summary>
        /// <param name="message">A human-readable description of the error condition.</param>
        public VisionNetException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionNetException"/> class with a specific error message and inner exception.
        /// </summary>
        /// <param name="message">A human-readable description of the error condition.</param>
        /// <param name="inner">The exception that caused the current exception to be thrown.</param>
        public VisionNetException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionNetException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="info"/> is <see langword="null"/>.</exception>
        protected VisionNetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResourceReferenceProperty = info.GetString("ResourceReferenceProperty");
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="info"/> is <see langword="null"/>.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            info.AddValue("ResourceReferenceProperty", ResourceReferenceProperty);
            base.GetObjectData(info, context);
        }
    }
}
