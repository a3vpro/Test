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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace VisionNet.Core.Serialization
{
    /// <summary>
    /// Implements a instance of serializer/deserializer using a JSON format
    /// </summary>
    public class JSONSerializer: Serializer
    {
        /// <inheritdoc/>
        public override void Serialize(Stream stream, object source, object format = null, object parameters = null)
        {
            var jsonSerializer = new JsonSerializer()
            {
                //TypeNameHandling = TypeNameHandling.Auto
                TypeNameHandling = TypeNameHandling.All
            };
            jsonSerializer.Converters.Add(new StringEnumConverter());

            using (var sr = new StreamWriter(stream))
            using (var jsonTextWriter = new JsonTextWriter(sr) { Formatting = Formatting.Indented })
            {
                jsonSerializer.Serialize(jsonTextWriter, source);
            }
        }

        /// <inheritdoc/>
        public override object Deserialize(Stream stream, Type type)
        {
            var jsonSerializer = new JsonSerializer()
            {
                TypeNameHandling = TypeNameHandling.Auto
                //TypeNameHandling = TypeNameHandling.All
            };
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return jsonSerializer.Deserialize(jsonTextReader, type);
            }
        }
    }
}
