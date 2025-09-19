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
    /// Base class of a serializer/deserializer instance
    /// </summary>
    public abstract class Serializer<TFormat> : ISerializer<TFormat>
    {
        /// <inheritdoc/>
        
        /// <summary> The Serialize function serializes an object to a stream.</summary>
        /// <param name="stream"> The stream to write the serialized object to.</param>
        /// <param name="source"> The source object to serialize.</param>
        /// <param name="format"> The format to use when serializing the object.</param>
        /// <param name="parameters"> The parameters object is used to pass additional information to the serializer. 
        /// this can be used for example, when using a custom &lt;see cref=&quot;t:system.runtime.serialization.iformatter&quot; /&gt; or &lt;see cref=&quot;t:system.xml.serialization&quot; /&gt; implementation that requires additional information in order to properly serialize an object.</param>
        /// <returns> The number of bytes written to the stream.</returns>
        public void Serialize<T>(Stream stream, T source, TFormat format = default(TFormat), object parameters = null)
        {
            Serialize(stream, (object)source, format, parameters);
        }

        /// <inheritdoc/>
        public abstract void Serialize(Stream stream, object source, TFormat format = default(TFormat), object parameters = null);

        /// <inheritdoc/>
        public T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(stream, typeof(T));
        }

        /// <inheritdoc/>
        public abstract object Deserialize(Stream stream, Type type);
    }
}
