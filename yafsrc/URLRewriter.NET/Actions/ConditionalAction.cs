// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2007 Intelligencia
// Copyright 2007 Seth Yates
// 

using System;
using System.Collections;
using Intelligencia.UrlRewriter.Conditions;

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
		public IList Conditions
		{
			get
			{
				return _conditions;
			}
		}

		/// <summary>
		/// Child rules.
		/// </summary>
		public IList Actions
		{
			get
			{
				return _actions;
			}
		}

		/// <summary>
		/// Determines if the action matches the current context.
		/// </summary>
		/// <param name="context">The context to match on.</param>
		/// <returns>True if the condition matches.</returns>
		public virtual bool IsMatch(RewriteContext context)
		{
			// Ensure the conditions are met.
			foreach (IRewriteCondition condition in Conditions)
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
			for (int i = 0; i < Actions.Count; i++)
			{
				IRewriteCondition condition = Actions[i] as IRewriteCondition;
				if (condition == null || condition.IsMatch(context))
				{
					IRewriteAction action = Actions[i] as IRewriteAction;
                    RewriteProcessing processing = action.Execute(context);
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
