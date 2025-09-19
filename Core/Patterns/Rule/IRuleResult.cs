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

namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Represents the result of a rule evaluation, including the action to be executed and the success status.
    /// </summary>
    public interface IRuleResult
    {
        /// <summary>
        /// Gets or sets the action that will be executed if the rule is considered successful.
        /// </summary>
        Action ActionToBeExecuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the rule evaluation was successful.
        /// </summary>
        bool IsSuccess { get; set; }

        /// <summary>
        /// Executes the action associated with the rule result.
        /// </summary>
        void Execute();
    }
}
