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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace VisionNet.Core.Serialization
{
    public class DirectJSonConverter<T> : JsonConverter
        where T: new()
    {
        // nuevo
        static readonly IContractResolver resolver = new NoTypeConverterContractResolver();
        class NoTypeConverterContractResolver : DefaultContractResolver
        {
            
            /// <summary> The CreateContract function is called by the JsonSerializer to create a contract for each type it encounters.
            /// This override allows us to return our own custom contract that will be used when serializing and deserializing objects of type T.</summary>
            /// <param name="objectType"> What is this?</param>
            /// <returns> A jsoncontract object.</returns>
            protected override JsonContract CreateContract(Type objectType)
            {
                if (typeof(T).IsAssignableFrom(objectType))
                {
                    var contract = this.CreateObjectContract(objectType);
                    contract.Converter = null; // Also null out the converter to prevent infinite recursion.
                    return contract;
                }
                return base.CreateContract(objectType);
            }
        }

        
        /// <summary> The CanConvert function is used to determine if the converter can convert a given type.
        /// This function is called by JsonSerializer when it needs to know if this converter can handle a given type.</summary>
        /// <param name="objectType"> The type of the object to convert.
        /// </param>
        /// <returns> True if the objecttype parameter is of type t or a derived class. otherwise, it returns false.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        
        /// <summary> The ReadJson function is used to deserialize JSON text into an object or value.</summary>
        /// <param name="reader"> The jsonreader to read from.</param>
        /// <param name="objectType"> The type of the object.</param>
        /// <param name="existingValue"> The existing value of object being read.</param>
        /// <param name="serializer"> The jsonserializer used to deserialize the object.</param>
        /// <returns> The item object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            T item = new T();
            serializer.ContractResolver = resolver;
            serializer.Populate(jo.CreateReader(), item);

            return item;
        }

        public override bool CanWrite => true;

        
        /// <summary> The WriteJson function is used to serialize an object into a JSON string.</summary>
        /// <param name="writer"> The jsonwriter writer is used to write the json data.
        /// </param>
        /// <param name="value"> The object to serialize.</param>
        /// <param name="serializer"> The jsonserializer is used to serialize the object.
        /// </param>
        /// <returns> A jsonwriter object.</returns>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ContractResolver = resolver;
            serializer.Serialize(writer, value);
        }
    }
}
