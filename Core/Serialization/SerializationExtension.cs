using System;
using System.IO;

namespace VisionNet.Core.Serialization
{
    public static class SerializationExtension
    {
        /// <summary>
        /// Serializes the specified <paramref name="instance"/> using the provided <typeparamref name="TSerializator"/>
        /// implementation and returns the ASCII-encoded payload produced by the serializer.
        /// </summary>
        /// <typeparam name="TSerializator">A serializer type that implements <see cref="ISerializer{TFormat}"/> for <see cref="object"/> and exposes a public parameterless constructor.</typeparam>
        /// <param name="instance">The non-null object instance to serialize. The underlying serializer must be capable of handling the runtime type of this object.</param>
        /// <param name="parameters">Optional serializer-specific parameters that are forwarded to the serializer implementation.</param>
        /// <returns>An ASCII string containing the serialized representation produced by <typeparamref name="TSerializator"/>. The returned value is never <see langword="null"/>, but may be empty when the serializer emits no content.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="instance"/> is <see langword="null"/>.</exception>
        public static string Serialize<TSerializator>(this object instance, object parameters = null)
            where TSerializator: ISerializer<object>, new()
        {
            if (instance == null)
                throw new ArgumentException($"Instance is not a valid serializable object {instance.GetType()}", nameof(instance));

            string result = string.Empty;

            using (var stream = new MemoryStream())
            {
                var serializator = new TSerializator();
                serializator.Serialize(stream, instance, typeof(object), parameters);
                result = System.Text.Encoding.ASCII.GetString(stream.ToArray());
                stream.Close();
            }

            return result;
        }

        /// <summary>
        /// Deserializes the specified ASCII <paramref name="serializedInstance"/> into an instance of type <typeparamref name="T"/>
        /// using the provided <typeparamref name="TSerializator"/> implementation.
        /// </summary>
        /// <typeparam name="T">The reference type to deserialize the payload into. The serializer must support this type.</typeparam>
        /// <typeparam name="TSerializator">A serializer type that implements <see cref="ISerializer{TFormat}"/> for <see cref="object"/> and exposes a public parameterless constructor.</typeparam>
        /// <param name="serializedInstance">The non-empty ASCII string produced by a compatible serializer.</param>
        /// <param name="parameters">Optional serializer-specific parameters reserved for implementations that require additional context.</param>
        /// <returns>An instance of <typeparamref name="T"/> produced by <typeparamref name="TSerializator"/>. The method returns <see langword="null"/> when the serializer yields a null reference.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="serializedInstance"/> is <see langword="null"/>, empty, or whitespace.</exception>
        public static T Deserialize<T, TSerializator>(this string serializedInstance, object parameters = null)
            where TSerializator : ISerializer<object>, new()
            where T: class
        {
            if (string.IsNullOrWhiteSpace(serializedInstance))
                throw new ArgumentException($"String is not a valid deserializable", nameof(serializedInstance));

            T obj = default(T);
            byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(serializedInstance);
            using (Stream stream = new MemoryStream(byteArray))
            {
                var serializator = new TSerializator();
                obj = serializator.Deserialize<T>(stream);
                stream.Close();
            }

            return obj;
        }
    }
}