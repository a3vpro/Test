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
    public class InvalidConfigurationParameterException : VisionNetException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase VisionNet.Devices.InvalidConfigurationParameterException.
        /// </summary>
        public InvalidConfigurationParameterException() { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase VisionNet.Devices.InvalidConfigurationParameterException con el mensaje de error especificado.
        /// </summary>
        /// <param name="message">Mensaje que describe el error</param>
        public InvalidConfigurationParameterException(string message): base(message) { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase VisionNet.Devices.InvalidConfigurationParameterException con el mensaje de
        /// error especificado y una referencia a la excepción interna que representa la
        /// causa de esta excepción.
        /// </summary>
        /// <param name="message">Mensaje de error que explica el motivo de la excepción.</param>
        /// <param name="innerException">
        /// Inicializa una nueva instancia de la clase System.Exception con el mensaje de
        /// error especificado y una referencia a la excepción interna que representa la
        /// causa de esta excepción.
        /// </param>
        public InvalidConfigurationParameterException(string message, Exception innerException): base(message, innerException) { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase VisionNet.Devices.InvalidConfigurationParameterException con datos serializados.
        /// </summary>
        /// <param name="info">
        /// System.Runtime.Serialization.SerializationInfo que contiene los datos serializados
        /// del objeto que hacen referencia a la excepción que se va a producir.
        /// </param>
        /// <param name="context">
        /// System.Runtime.Serialization.StreamingContext que contiene información contextual
        /// sobre el origen o el destino.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null.</exception>  
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or System.Exception.HResult is zero (0).</exception>  
        [SecuritySafeCritical]
        protected InvalidConfigurationParameterException(SerializationInfo info, StreamingContext context): base(info, context) { }
    }
}
