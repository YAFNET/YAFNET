namespace ServiceStack.FluentValidation.Internal {
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using Validators;

	/// <summary>
	/// Selects validators that belong to the specified rulesets.
	/// </summary>
	public class RulesetValidatorSelector : IValidatorSelector {
        /// <summary>
		/// Rule sets
		/// </summary>
		public string[] RuleSets { get; }

        /// <summary>
		/// Creates a new instance of the RulesetValidatorSelector.
		/// </summary>
		public RulesetValidatorSelector(params string[] rulesetsToExecute) {
			this.RuleSets = rulesetsToExecute;
		}

		/// <summary>
		/// Determines whether or not a rule should execute.
		/// </summary>
		/// <param name="rule">The rule</param>
		/// <param name="propertyPath">Property path (eg Customer.Address.Line1)</param>
		/// <param name="context">Contextual information</param>
		/// <returns>Whether or not the validator can execute.</returns>
		public bool CanExecute(IValidationRule rule, string propertyPath, ValidationContext context) {
			if (string.IsNullOrEmpty(rule.RuleSet) && this.RuleSets.Length > 0) {
				if (IsIncludeRule(rule)) {
					return true;
				}
			}

			if (string.IsNullOrEmpty(rule.RuleSet) && this.RuleSets.Length == 0) return true;
			if (string.IsNullOrEmpty(rule.RuleSet) && this.RuleSets.Length > 0 && this.RuleSets.Contains("default", StringComparer.OrdinalIgnoreCase)) return true;
			if (!string.IsNullOrEmpty(rule.RuleSet) && this.RuleSets.Length > 0 && this.RuleSets.Contains(rule.RuleSet)) return true;
			if (this.RuleSets.Contains("*")) return true;

			return false;
		}

		/// <summary>
		/// Checks if the rule is an IncludeRule
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		protected bool IsIncludeRule(IValidationRule rule) {
			return rule is IncludeRule;
		}
	}
}