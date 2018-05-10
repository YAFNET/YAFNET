// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System.Collections;

namespace Intelligencia.UrlRewriter.Actions
{
	/// <summary>
	/// A Conditional Action
	/// </summary>
	public class ConditionalAction : IRewriteAction, IRewriteCondition
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConditionalAction()
		{
		}

		/// <summary>
		/// Conditions that must hold for the rule to fire.
		/// </summary>
		public IList Conditions => this._conditions;

	    /// <summary>
		/// Child rules.
		/// </summary>
		public IList Actions => this._actions;

	    /// <summary>
		/// Determines if the action matches the current context.
		/// </summary>
		/// <param name="context">The context to match on.</param>
		/// <returns>True if the condition matches.</returns>
		public virtual bool IsMatch(RewriteContext context)
		{
			// Ensure the conditions are met.
			foreach (IRewriteCondition condition in this.Conditions)
			{
				if (!condition.IsMatch(context))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Executes the rule.
		/// </summary>
		/// <param name="context"></param>
        public virtual RewriteProcessing Execute(RewriteContext context)
		{
			// Execute the actions.
			for (var i = 0; i < this.Actions.Count; i++)
			{
				var condition = this.Actions[i] as IRewriteCondition;
				if (condition == null || condition.IsMatch(context))
				{
					var action = this.Actions[i] as IRewriteAction;
                    var processing = action.Execute(context);
					if (processing != RewriteProcessing.ContinueProcessing)
					{
						return processing;
					}
				}
			}

            return RewriteProcessing.ContinueProcessing;
		}

		private ArrayList _actions = new ArrayList();
		private ArrayList _conditions = new ArrayList();
	}
}
