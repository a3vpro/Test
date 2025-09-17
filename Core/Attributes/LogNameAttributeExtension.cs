using System.Linq;

namespace VisionNet.Logging
{
    /// <summary>
    /// Provides extension helpers for retrieving <see cref="LogNameAttribute"/> metadata from
    /// consumer types.
    /// </summary>
    public static class LogNameAttributeExtension
    {

        /// <summary>
        /// Retrieves the configured log name from the <see cref="LogNameAttribute"/> applied to the
        /// object's runtime type.
        /// </summary>
        /// <param name="obj">The instance whose type is expected to be decorated with a <see cref="LogNameAttribute"/>.</param>
        /// <returns>The value supplied to <see cref="LogNameAttribute.LogName"/>, or <see langword="null"/> when the attribute is absent.</returns>
        public static string GetLogName(this object obj)
        {

            var attribute = obj.GetType()
                .GetCustomAttributes(true)
                .OfType<LogNameAttribute>()
                .FirstOrDefault();

            if (attribute != null)
            {
                return attribute.LogName;
            }
            
            return null;
        }
    }
}
