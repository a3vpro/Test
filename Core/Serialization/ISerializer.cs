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
using System.IO;

namespace VisionNet.Core.Serialization
{
    /// <summary>
    /// Specifies a serializer/deserializer instance
    /// </summary>
    public interface ISerializer<TFormat>
    {
        /// <summary>
        /// Execute a serialization of a instance into a stream
        /// </summary>
        /// <typeparam name="T">Associated type</typeparam>
        /// <param name="stream">Destination of the serialization</param>
        /// <param name="source">Source instance to serialize</param>
        void Serialize<T>(Stream stream, T source, TFormat format, object parameters);

        /// <summary>
        /// Execute a serialization of a instance into a stream
        /// </summary>
        /// <param name="stream">Destination of the serialization</param>
        /// <param name="source">Source instance to serialize</param>
        void Serialize(Stream stream, object source, TFormat format, object parameters);

        /// <summary>
        /// Execute a deserialization of a stream into a instance
        /// </summary>
        /// <typeparam name="T">Associated type</typeparam>
        /// <param name="stream">Source of the serialization</param>
        /// <returns>Result object of the serialization</returns>
        T Deserialize<T>(Stream stream);

        /// <summary>
        /// Execute a deserialization of a stream into a instance
        /// </summary>
        /// <param name="stream">Source of the serialization</param>
        /// <param name="type">Type of the destination instance</param>
        /// <returns>Result object of the serialization</returns>
        object Deserialize(Stream stream, Type type);
    }
}
