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
    /// Represents a rule that always evaluates as successful and executes the provided action.
    /// </summary>
    /// <typeparam name="R">The type of rule result, which must implement the <see cref="IRuleResult"/> interface.</typeparam>
    public class NoRule<R> : Rule<R> where R : IRuleResult, new()
    {
        /// <summary> The NoRule function is a constructor for the NoRule class. It takes in an Action and passes it to the base Rule class.</summary>
        /// <param name="actionToBeExecuted"> The action to be executed.
        /// </param>
        /// <returns> The value of the actiontobeexecuted.</returns>
        public NoRule(Action actionToBeExecuted) : base(actionToBeExecuted) { }


        /// <summary> The NoRule function is a constructor for the NoRule class. It does not take any parameters, and it does not return anything.</summary>
        /// <returns> A rule object.</returns>
        public NoRule() : base() { }


        /// <summary> The Evaluate function is the main function of a rule. It returns an R object, which contains information about whether or not the rule was successful.</summary>
        /// <returns> An r object</returns>
        public override R Evaluate()
        {
            return new R() { IsSuccess = true };
        }
    }

    /// <summary>
    /// Represents a rule that always evaluates as successful and executes the provided action, with a specific rule result type of <see cref="RuleResult"/>.
    /// </summary>
    public class NoRule : NoRule<RuleResult>
    {
        /// <summary> The NoRule function is a constructor for the NoRule class. It takes in an Action and passes it to the base Rule class.</summary>
        /// <param name="actionToBeExecuted"> The action to be executed when the rule is satisfied.
        /// </param>
        /// <returns> A boolean value of false</returns>
        public NoRule(Action actionToBeExecuted) : base(actionToBeExecuted) { }


        /// <summary> The NoRule function is a constructor for the NoRule class. It does not take any parameters, and it does not return anything.</summary>
        /// <returns> A string.</returns>
        public NoRule() : base() { }
    }
}
