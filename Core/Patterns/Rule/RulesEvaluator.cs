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
using System.Collections.Generic;

namespace VisionNet.Core.Patterns
{

    /// <summary>
    /// Evaluates a set of rules and determines the results based on the given conditions.
    /// </summary>
    /// <typeparam name="R">The type of rule result, which must inherit from <see cref="RuleResult"/>.</typeparam>
    public class RulesEvaluator<R>
        where R : RuleResult, new()
    {
        private readonly List<RuleChain<R>> _rulesChains = new List<RuleChain<R>>();


        /// <summary> The Add function adds a rule to the RuleChain.
        /// The Add function returns an instance of the RuleChain class, which allows you to chain rules together.&lt;/para&gt;</summary>
        /// <param name="rule">The rule to be added.</param>
        /// <returns> The rulechain object</returns>
        public RuleChain<R> Add(IRule<R> rule)
        {
            var ifRule = new RuleChain<R>(rule);
            _rulesChains.Add(ifRule);
            return ifRule;
        }


        /// <summary> The Evaluate function is used to evaluate the rules chains.</summary>
        /// <returns> A list of rules that were evaluated as true.</returns>
        public void Evaluate()
        {
            foreach (var ruleChain in _rulesChains)
                ruleChain.Evaluate();
        }
    }

    /// <summary>
    /// Evaluates a set of rules and determines the results based on the given conditions, with a specific rule result type of <see cref="RuleResult"/>.
    /// </summary>
    public class RulesEvaluator : RulesEvaluator<RuleResult>
    {
    }

}
