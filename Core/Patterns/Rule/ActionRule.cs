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
    /// Represents a rule with an action that is executed when a condition is met.
    /// </summary>
    /// <typeparam name="R">The type of rule result, which must implement the IRuleResult interface.</typeparam>
    public class ActionRule<R> : Rule<R> where R : IRuleResult, new()
    {
        /// <summary>
        /// Gets or sets the condition that must be satisfied for the action to be executed.
        /// </summary>
        public Func<bool> Condition { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRule{R}"/> class with an action and a condition.
        /// </summary>
        /// <param name="actionToBeExecuted">The action that will be executed if the condition evaluates to true.</param>
        /// <param name="condition">The condition that must return true for the action to be executed.</param>
        public ActionRule(Action actionToBeExecuted, Func<bool> condition)
            : base(actionToBeExecuted)
        {
            Condition = condition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRule{R}"/> class with default values.
        /// </summary>
        public ActionRule()
            : base()
        {
        }

        /// <summary>
        /// Evaluates the condition of the rule. If the condition evaluates to true, the rule is considered successful.
        /// </summary>
        /// <returns>A rule result object indicating the success or failure of the rule.</returns>
        public override R Evaluate()
        {
            _ruleResult.IsSuccess = Condition == null ? false : Condition();
            return _ruleResult;
        }
    }

    /// <summary>
    /// Represents a rule with an action that is executed when a condition is met, with a specific rule result type.
    /// </summary>
    public class ActionRule : ActionRule<RuleResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRule"/> class with an action and a condition.
        /// </summary>
        /// <param name="actionToBeExecuted">The action that will be executed if the condition evaluates to true.</param>
        /// <param name="condition">The condition that must return true for the action to be executed.</param>
        public ActionRule(Action actionToBeExecuted, Func<bool> condition)
            : base(actionToBeExecuted, condition)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionRule"/> class with default values.
        /// </summary>
        public ActionRule()
            : base()
        {
        }
    }
}