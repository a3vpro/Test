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
    public class RuleResult: IRuleResult
    {
        /// <summary>
        /// Gets or sets the action that will be executed if the rule is considered successful.
        /// </summary>
        public Action ActionToBeExecuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the rule evaluation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary> The RuleResult function is used to determine whether a rule has been violated or not.
        /// If the rule has been violated, then the RuleResult function will return true.
        /// If the rule has not been violated, then the RuleResult function will return false.</summary>
        /// <returns> A ruleresult object.</returns>
        public RuleResult()
        {
        }
        
        /// <summary> The RuleResult function is used to execute a rule and return the result of that execution.</summary>
        /// <param name="actionToBeExecuted"> What is this parameter used for?
        /// </param>
        /// <returns> The actiontobeexecuted.</returns>
        public RuleResult(Action actionToBeExecuted)
        {
            ActionToBeExecuted = actionToBeExecuted;
        }

        /// <summary> The Execute function is used to execute the ActionToBeExecuted if IsSuccess is true.
        /// </summary>
        /// <returns> A boolean value. if the command is executed successfully, it returns true, otherwise false.</returns>
        public virtual void Execute()
        {
            if (IsSuccess)
                ActionToBeExecuted?.Invoke();
        }
    }
}
