using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Defines the interface for handling and displaying exceptions in a dialog box.
    /// </summary>
    public interface IErrorBox
    {
        /// <summary>
        /// Displays an exception message, optionally in a dialog box.
        /// </summary>
        /// <param name="dialog">Indicates whether the exception should be displayed in a dialog box.</param>
        /// <param name="exception">The exception containing detailed error information.</param>
        /// <param name="shortMessage">A brief description of the exception.</param>
        void ShowException(bool dialog, Exception exception, string shortMessage = "");

        /// <summary>
        /// Displays the informational message
        /// </summary>
        /// <param name="information">Information to display</param>
        /// <param name="callOrder">Order of the call in the stack</param>
        void ShowInfo(bool dialog, string information, int callOrder, string shortMessage = "");
    }
}

