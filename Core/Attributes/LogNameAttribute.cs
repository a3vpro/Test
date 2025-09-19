using System;

namespace VisionNet.Logging
{
    /// <summary>
    /// Specifies the log name that should be associated with the decorated class when emitting log entries.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LogNameAttribute: Attribute
    {
        /// <summary>
        /// Represents the log name assigned through the attribute to categorize log output.
        /// </summary>
        public string LogName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogNameAttribute"/> class using the provided log name identifier.
        /// </summary>
        /// <param name="logName">Log identifier to associate with the decorated class when logging.</param>
        public LogNameAttribute(string logName)
        {
            LogName = logName;
        }
    }  
}
