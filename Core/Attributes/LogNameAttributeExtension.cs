using System.Linq;

namespace VisionNet.Logging
{
    public static class LogNameAttributeExtension
    {
        
        /// <summary> The GetLogName function returns the name of a log file based on the LogNameAttribute.
        /// If no attribute is found, it returns null.</summary>
        /// <param name="this object obj"> The object to get the log name for.</param>
        /// <returns> The value of the logname property in the lognameattribute class</returns>
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
