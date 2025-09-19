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
using VisionNet.Core.Abstractions;

namespace VisionNet.Core.States
{
    /// <summary>
    /// Represents the execution lifecycle validation rules, defining which <see cref="ExecutionStatus"/> values can transition between each other.
    /// </summary>
    public class ExecutionStatusTransition : StatusValidator<ExecutionStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionStatusTransition"/> class with the supported execution status transitions.
        /// Ensures the workflow can only progress from <see cref="ExecutionStatus.Initial"/> to <see cref="ExecutionStatus.Ready"/>,
        /// then to <see cref="ExecutionStatus.Executing"/>, and subsequently to <see cref="ExecutionStatus.Finished"/>,
        /// <see cref="ExecutionStatus.Error"/>, or <see cref="ExecutionStatus.Aborted"/>. Completed, errored, or aborted executions
        /// may be retried by transitioning back to <see cref="ExecutionStatus.Executing"/>.
        /// </summary>
        public ExecutionStatusTransition()
        {
            AddTransition(ExecutionStatus.Initial, ExecutionStatus.Ready);

            AddTransition(ExecutionStatus.Ready, ExecutionStatus.Executing);

            AddTransition(ExecutionStatus.Executing, ExecutionStatus.Finished);
            AddTransition(ExecutionStatus.Executing, ExecutionStatus.Error);
            AddTransition(ExecutionStatus.Executing, ExecutionStatus.Aborted);

            AddTransition(ExecutionStatus.Finished, ExecutionStatus.Executing);
            AddTransition(ExecutionStatus.Error, ExecutionStatus.Executing);
            AddTransition(ExecutionStatus.Aborted, ExecutionStatus.Executing);
        }
    }
}
