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
using System.Threading;
using System.Threading.Tasks;

namespace VisionNet.Core.Abstractions
{
    /// <summary>
    /// Specifies a executable instance
    /// </summary>
    public interface IAsyncExecutable: IExecutable
    {
        /// <summary>
        /// Perform the execute command in asynchronous mode
        /// </summary>
        Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
