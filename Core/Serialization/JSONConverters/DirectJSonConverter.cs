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
    /// Provides a JSON converter that directly creates and populates instances of <typeparamref name="T"/>,
    /// bypassing any registered type converters to avoid recursion when deserializing.
    /// </summary>
    /// <typeparam name="T">The target type that must expose a public parameterless constructor.</typeparam>
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
        /// Determines whether the converter can convert instances of the provided <paramref name="objectType"/>.
        /// </summary>
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
        /// Creates a new instance of <typeparamref name="T"/> and populates it with values read from the provided JSON reader.
        /// </summary>
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
        /// Gets a value indicating that the converter supports writing so that the serializer will invoke
        /// <see cref="WriteJson(JsonWriter, object, JsonSerializer)"/> when serializing instances of <typeparamref name="T"/>.
        /// </summary>
        public override bool CanWrite => true;


        /// <summary>
        /// Serializes the supplied <paramref name="value"/> by delegating to the configured serializer while enforcing the
        /// converter's contract resolver to bypass type converters during serialization.
        /// </summary>
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
