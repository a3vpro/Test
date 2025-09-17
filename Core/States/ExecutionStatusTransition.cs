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
    public class ExecutionStatusTransition : StatusValidator<ExecutionStatus>
    {
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
