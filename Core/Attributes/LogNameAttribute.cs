using System;

namespace VisionNet.Logging
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogNameAttribute: Attribute
    {
        public string LogName;

        
        /// <summary> The LogNameAttribute function is used to set the name of a log file.</summary>
        /// <param name="string logName"> The name of the log file.</param>
        /// <returns> The name of the log.</returns>
        public LogNameAttribute(string logName)
        {
            LogName = logName;
        }
    }  
}
