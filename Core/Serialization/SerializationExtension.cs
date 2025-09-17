using System;
using System.IO;

namespace VisionNet.Core.Serialization
{
    public static class SerializationExtension
    {
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