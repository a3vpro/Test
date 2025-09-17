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
namespace VisionNet.Core.Patterns
{
    /// <summary>
    /// Represents a chain of rules that can be evaluated in sequence, with conditional rules for "if" and "else" logic.
    /// </summary>
    /// <typeparam name="R">The type of rule result, which must inherit from <see cref="RuleResult"/>.</typeparam>
    public class RuleChain<R>
        where R : RuleResult, new()
    {
        private readonly IRule<R> _rule;

        /// <summary>
        /// Gets the next rule in the chain that is executed if the current rule evaluates to true.
        /// </summary>
        public RuleChain<R> _ifRule { get; private set; }

        /// <summary>
        /// Gets the next rule in the chain that is executed if the current rule evaluates to false.
        /// </summary>
        public RuleChain<R> _elseRule { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this rule is the last rule in the chain.
        /// </summary>
        public bool IsLastInChain => (_ifRule != null) || (_elseRule != null);

        /// <summary> The RuleChain function allows you to chain together multiple rules,
        /// so that the output of one rule is passed as input to another.  This can be
        /// used for a variety of purposes, such as combining multiple rules into a single
        /// rule (e.g., if you have two separate rules that each take an integer and return 
        /// an integer), or for creating more complex logic by passing the output from one
        /// rule into another.</summary>
        /// <param name="mainRule"> </param>
        /// <returns> The rulechain object.</returns>
        public RuleChain(IRule<R> mainRule)
        {
            _rule = mainRule;
        }

        /// <summary> The AddIf function adds a rule to the chain that will only be executed if the previous rules in the chain are successful.</summary>
        ///
        /// <param name="rule"> What is this parameter used for?</param>
        ///
        /// <returns> The _ifrule object</returns>
        public RuleChain<R> AddIf(IRule<R> rule)
        {
            _ifRule = new RuleChain<R>(rule);
            return _ifRule;
        }

        /// <summary> The AddElse function adds a rule to the chain that will be executed if all previous rules in the chain return false.</summary>
        /// <param name="rule"> The rule to be executed if the condition is true.
        /// </param>
        /// <returns> A new rulechain object</returns>
        public RuleChain<R> AddElse(IRule<R> rule)
        {
            _elseRule = new RuleChain<R>(rule);
            return _elseRule;
        }

        /// <summary> The Evaluate function evaluates the rule and executes it if it is successful. If the rule is not successful, then the elseRule will be evaluated.</summary>
        /// <returns> A ruleresult object</returns>
        public void Evaluate()
        {
            var ruleResult = _rule.Evaluate();
            if (ruleResult.IsSuccess)
            {
                ruleResult.Execute();
                _ifRule?.Evaluate();
            }
            else
                _elseRule?.Evaluate();
        }
    }

    /// <summary>
    /// Represents a chain of rules that can be evaluated in sequence, with conditional rules for "if" and "else" logic, with a specific rule result type of <see cref="RuleResult"/>.
    /// </summary>
    public class RuleChain : RuleChain<RuleResult>
    {
        /// <summary> The RuleChain function is a constructor that takes in an IRule object and passes it to the base class.
        /// The RuleChain function also has a public method called AddRule which adds rules to the chain.</summary>
        /// <param name="mainRule"> The main rule that will be executed.
        /// </param>
        /// <returns> A rulechain object.</returns>
        public RuleChain(IRule<RuleResult> mainRule)
            : base(mainRule)
        {
        }
    }
}
