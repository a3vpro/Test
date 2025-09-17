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
namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Specifies a executable instance
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// The state of execution
        /// </summary>
        ExecutionStatus Status { get; }

        /// <summary>
        /// Perform the execute command
        /// </summary>
        void Execute();

        /// <summary>
        /// Perform the execute command
        /// </summary>
        /// <returns>True if action is successful</returns>
        bool TryExecute();
    }
}
