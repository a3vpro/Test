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
using System;
using System.Collections.Generic;
using System.Threading;

namespace VisionNet.Core.Tasks
{
    public class SequenceItem
    {
        /// <summary>
        /// Gets or sets the delay, in milliseconds, to wait before invoking the sequence action.
        /// Consumers should provide a non-negative value to prevent premature execution.
        /// </summary>
        public int DelayMs { get; set; } = 20;

        /// <summary>
        /// Gets or sets the parameter collection supplied to the sequence action; keys are parameter names and values are the associated payloads.
        /// Each entry should match the expectations of the configured action to avoid runtime errors.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the action that executes the sequence step, accepting the configured parameters and a cancellation token source to observe or trigger cancellation.
        /// The delegate should handle cancellation cooperatively and validate the provided parameter set.
        /// </summary>
        public Action<Dictionary<string, object>, CancellationTokenSource> Action { get; set; }
    }
}
