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
    /// Represents an abstract base class for defining rules that can be evaluated to produce a result.
    /// </summary>
    /// <typeparam name="R">The type of rule result, which must implement the <see cref="IRuleResult"/> interface.</typeparam>
    public abstract class Rule<R> : IRule<R> where R : IRuleResult, new()
    {
        protected readonly R _ruleResult;

        /// <summary> The Rule function is a constructor for the Rule class. It takes an Action as its parameter and assigns it to the _ruleResult object's ActionToBeExecuted property.</summary>
        /// <param name="actionToBeExecuted"> The action to be executed when the rule is met.
        /// </param>
        /// <returns> A ruleresult object</returns>
        public Rule(Action actionToBeExecuted)
        {
            _ruleResult = new R() { ActionToBeExecuted = actionToBeExecuted };
        }

        /// <summary> The Rule function is a function that takes in an input of type T and returns a result of type R.</summary>
        /// <returns> The result of the rule</returns>
        public Rule()
        {
            _ruleResult = new R();
        }

        public abstract R Evaluate();
    }

    /// <summary>
    /// Represents an abstract base class for defining rules that can be evaluated to produce a result, with a specific rule result type of <see cref="RuleResult"/>.
    /// </summary>
    public abstract class Rule : Rule<RuleResult>, IRule
    {
        /// <summary> The Rule function is a constructor that takes an Action as its parameter.
        /// The Rule function then calls the base class's constructor, passing in the actionToBeExecuted parameter.</summary>
        /// <param name="actionToBeExecuted"> The action to be executed.
        /// </param>
        /// <returns> A boolean value</returns>
        public Rule(Action actionToBeExecuted)
            : base(actionToBeExecuted)
        {
        }

        /// <summary> The Rule function is a constructor for the Rule class. It takes no parameters and returns nothing.</summary>
        /// <returns> A string value.</returns>
        public Rule()
            : base()
        {
        }
    }
}
