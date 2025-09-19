using System;
using System.Collections.Generic;
using System.Text;
using VisionNet.Core.Abstractions;
using VisionNet.Core.Types;

namespace VisionNet.Vision.Core
{
    /// <summary>
    /// Specifies a executable instance
    /// </summary>
    public interface IInspectionFunctionExecutable
    {
        /// <summary>
        /// The state of execution
        /// </summary>
        ExecutionStatus Status { get; }

        /// <summary>
        /// Perform the execute command
        /// </summary>
        InspectionResult Execute(IInputParametersCollection inputParameters, List<NamedValue> step);
    }
}
