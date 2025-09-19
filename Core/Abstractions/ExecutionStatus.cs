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
    /// State of the service
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// Initial state. Unprepared
        /// </summary>
        Initial,
        /// <summary>
        /// The procedure is ready to execute
        /// </summary>
        Ready,
        /// <summary>
        /// The execution is running
        /// </summary>
        Executing,
        /// <summary>
        /// The execution is aborted
        /// </summary>
        Aborted,
        /// <summary>
        /// The execution is finished unsuccessfully
        /// </summary>
        Error,
        /// <summary>
        /// The procedure is completly finished
        /// </summary>
        Finished
    }
}
