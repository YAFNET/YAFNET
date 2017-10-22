// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;

namespace Intelligencia.UrlRewriter.Actions
{
    /// <summary>
    /// A Conditional Action
    /// </summary>
    public class ConditionalAction : IRewriteAction, IRewriteCondition
    {
        /// <summary>
        /// Conditions that must hold for the rule to fire.
        /// </summary>
        public IList<IRewriteCondition> Conditions
        {
            get { return _conditions; }
        }

        /// <summary>
        /// Child rules.
        /// </summary>
        public IList<IRewriteAction> Actions
        {
            get { return _actions; }
        }

        /// <summary>
        /// Determines if the action matches the current context.
        /// </summary>
        /// <param name="context">The context to match on.</param>
        /// <returns>True if the condition matches.</returns>
        public virtual bool IsMatch(IRewriteContext context)
        {
            return Conditions.IsMatch(context);
        }

        /// <summary>
        /// Executes the rule.
        /// </summary>
        /// <param name="context">The rewrite context</param>
        public virtual RewriteProcessing Execute(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // Execute the actions.
            for (int i = 0; i < Actions.Count; i++)
            {
                IRewriteCondition condition = Actions[i] as IRewriteCondition;
                if (condition == null || condition.IsMatch(context))
                {
                    IRewriteAction action = Actions[i];
                    RewriteProcessing processing = action.Execute(context);
                    if (processing != RewriteProcessing.ContinueProcessing)
                    {
                        return processing;
                    }
                }
            }

            return RewriteProcessing.ContinueProcessing;
        }

        private IList<IRewriteAction> _actions = new List<IRewriteAction>();
        private IList<IRewriteCondition> _conditions = new List<IRewriteCondition>();
    }
}
