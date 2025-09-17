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
    /// Image conversion exception.
    /// </summary>
    [Serializable]
    public class VisionNetException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionNetException"/> class.
        /// </summary>
        public VisionNetException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionNetException"/> class.
        /// </summary>
        /// <param name="message">Message providing some additional information.</param>
        public VisionNetException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionNetException"/> class.
        /// </summary>
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public VisionNetException(string message, Exception inner) : base(message, inner) { }

        ///<summary> 
        /// Initializes a new instance of the<see cref="VisionNetException"/> class. 
        ///</summary> 
        ///<param name = "info" > The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param> 
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected VisionNetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResourceReferenceProperty = info.GetString("ResourceReferenceProperty");
        }

        ///<summary> 
        /// When overridden in a derived class, sets the<see cref="SerializationInfo"/> with information about the exception. 
        ///</summary> 
        ///<param name = "info"> The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param> 
        ///<param name = "context"> The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param> 
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
