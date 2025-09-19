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
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;

namespace VisionNet.Core.Serialization
{
    class PointJSONConverter : JsonConverter
    {
        
        /// <summary> The WriteJson function is used to serialize a Point object into JSON.</summary>
        /// <param name="writer"> The jsonwriter to write the json data to.</param>
        /// <param name="value"> The value to convert.</param>
        /// <param name="serializer"> This is the serializer that will be used to convert the object.</param>
        /// <returns> A jobject that contains the x and y properties of the point object.</returns>
        public override void WriteJson(
            JsonWriter writer, object value, JsonSerializer serializer)
        {
            var point = (Point)value;

            serializer.Serialize(
                writer, new JObject { { "X", point.X }, { "Y", point.Y } });
        }

        
        /// <summary> The ReadJson function is used to deserialize a JSON object into an instance of the Point class.
        /// The JsonReader parameter provides access to the raw JSON data being deserialized, and the JsonSerializer 
        /// parameter allows us to read child values from it.</summary>
        /// <param name="reader"> The reader.</param>
        /// <param name="objectType"> The type of the object.</param>
        /// <param name="existingValue"> The existing value of object being read. if there is no existing value then null will be used.</param>
        /// <param name="serializer"> The jsonserializer is used to deserialize the json object into a jobject. 
        /// </param>
        /// <returns> A new point object with the x and y values from the json.</returns>
        public override object ReadJson(
            JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jObject = serializer.Deserialize<JObject>(reader);

            return new Point((int)jObject["X"], (int)jObject["Y"]);
        }

        
        /// <summary> The CanConvert function is used to determine whether or not the converter can convert a given type.
        /// This function is called by the JsonSerializer when it encounters an object that needs conversion.</summary>
        /// <param name="objectType"> The type of the object to convert.</param>
        /// <returns> True if the objecttype is of type point.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Point);
        }
    }
}
