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
using System.Xml.Serialization;

namespace VisionNet.Core.Serialization
{
    /// <summary>
    /// Implements a instance of serializer/deserializer using a JSON format
    /// </summary>
    public class XMLSerializer : Serializer
    {
        /// <inheritdoc/>
        
        /// <summary> The Serialize function serializes an object into a stream.</summary>
        /// <param name="stream"> The stream to which the object is serialized.</param>
        /// <param name="source"> The object to be serialized.</param>
        /// <param name="format"> What is this parameter used for?</param>
        /// <param name="parameters"> What is this parameter used for?</param>
        /// <returns> The serialized object.</returns>
        public override void Serialize(Stream stream, object source, object format = null, object parameters = null)
        {
            XmlSerializer xMLSerializer = new XmlSerializer(source.GetType());
            xMLSerializer.Serialize(stream, source);
        }

        /// <inheritdoc/>
        
        /// <summary> The Deserialize function takes a stream and type as parameters.
        /// It then creates an XmlSerializer object with the given type, and returns the deserialized object.</summary>
        /// <param name="stream"> The stream that contains the data to deserialize.</param>
        /// <param name="type"> The type of the object to deserialize.</param>
        /// <returns> The xml data in the form of an object.</returns>
        public override object Deserialize(Stream stream, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            return serializer.Deserialize(stream);
        }
    }
}
