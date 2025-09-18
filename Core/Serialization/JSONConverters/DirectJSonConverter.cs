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
    /// <summary>
    /// Provides a Json.NET converter that performs direct serialization and deserialization for <typeparamref name="T"/> while bypassing any
    /// type converters that could introduce metadata or interfere with the raw JSON payload.
    /// </summary>
    /// <typeparam name="T">The concrete type that will be populated from and written to JSON when this converter is applied.</typeparam>
    /// <remarks>
    /// Apply this converter when the default Json.NET behavior would otherwise add type information or invoke registered type converters that are
    /// not desired for <typeparamref name="T"/> instances. The converter ensures that objects of compatible types are populated directly using the
    /// standard contract resolver configured in <see cref="JsonSerializer"/>.
    /// </remarks>
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

        
        /// <summary>
        /// This function is called by JsonSerializer when it needs to know if this converter can handle a given type.</summary>
        /// <param name="objectType"> The type of the object to convert.
        /// </param>
        /// <returns> True if the objecttype parameter is of type t or a derived class. otherwise, it returns false.</returns>
        /// Determines whether the converter supports the specified object type.
        /// Determines whether the converter can convert instances of the provided <paramref name="objectType"/>.
        /// </summary>
        /// <param name="objectType">The object type being queried for converter support.</param>
        /// <returns><see langword="true"/> when the type is <typeparamref name="T"/> or a derived type; otherwise, <see langword="false"/>.</returns>
        /// <param name="objectType">The runtime type that the serializer is attempting to convert.</param>
        /// <returns>
        /// <see langword="true"/> when <typeparamref name="T"/> is assignable from <paramref name="objectType"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }


        /// <summary>
        /// <param name="reader"> The jsonreader to read from.</param>
        /// <param name="objectType"> The type of the object.</param>
        /// <param name="existingValue"> The existing value of object being read.</param>
        /// <param name="serializer"> The jsonserializer used to deserialize the object.</param>
        /// <returns> The item object.</returns>
        /// Populates a new <typeparamref name="T"/> instance from the JSON provided by the <paramref name="reader"/> using the configured serializer.
        /// Creates a new instance of <typeparamref name="T"/> and populates it with values read from the provided JSON reader.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> positioned at the start of the JSON object to deserialize.</param>
        /// <param name="objectType">The expected object type being created by the serializer.</param>
        /// <param name="existingValue">The current value of the object, which is ignored because the converter creates a new instance.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> that supplies settings and populates the instance.</param>
        /// <returns>A fully populated <typeparamref name="T"/> instance created from the JSON content.</returns>
        /// <param name="reader">The <see cref="JsonReader"/> that provides the JSON payload.</param>
        /// <param name="objectType">The target CLR type for the deserialization request.</param>
        /// <param name="existingValue">An existing value supplied by the serializer; ignored by this converter.</param>
        /// <param name="serializer">The serializer orchestrating the deserialization process.</param>
        /// <returns>A fully populated <typeparamref name="T"/> instance.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            T item = new T();
            serializer.ContractResolver = resolver;
            serializer.Populate(jo.CreateReader(), item);

            return item;
        }

        /// <summary>
        /// Gets a value indicating whether the converter participates in writing JSON.
        /// Gets a value indicating that the converter supports writing so that the serializer will invoke
        /// <see cref="WriteJson(JsonWriter, object, JsonSerializer)"/> when serializing instances of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// Always returns <see langword="true"/> so that <see cref="WriteJson(JsonWriter, object, JsonSerializer)"/> can serialize <typeparamref name="T"/>
        /// instances using the same contract resolver logic that is applied during deserialization.
        /// </remarks>
        public override bool CanWrite => true;


        /// <summary>
        /// <param name="writer"> The jsonwriter writer is used to write the json data.
        /// </param>
        /// <param name="value"> The object to serialize.</param>
        /// <param name="serializer"> The jsonserializer is used to serialize the object.
        /// </param>
        /// <returns> A jsonwriter object.</returns>
        /// Serializes an object to JSON using the supplied <paramref name="serializer"/> and the contract resolver configured for this converter.
        /// Serializes the supplied <paramref name="value"/> by delegating to the configured serializer while enforcing the
        /// converter's contract resolver to bypass type converters during serialization.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> that receives the serialized JSON content.</param>
        /// <param name="value">The <typeparamref name="T"/> instance to serialize.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> responsible for writing JSON data.</param>
        /// <param name="writer">The <see cref="JsonWriter"/> receiving the JSON output.</param>
        /// <param name="value">The object instance to serialize.</param>
        /// <param name="serializer">The serializer orchestrating the serialization process.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ContractResolver = resolver;
            serializer.Serialize(writer, value);
        }
    }
}
