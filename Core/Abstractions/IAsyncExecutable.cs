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
    /// Augments an executable component with asynchronous execution capabilities that support cooperative cancellation.
    /// </summary>
    public interface IAsyncExecutable: IExecutable
    {
        /// <summary>
        /// Performs the execution logic asynchronously, allowing the caller to await completion while optionally observing a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">Token used to observe cancellation requests; callers should provide a non-default value when they need to abort execution early.</param>
        /// <returns>A task that completes when the execution logic finishes or faults, and faults if the underlying operation fails.</returns>
        /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is signaled before or during execution.</exception>
        Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
