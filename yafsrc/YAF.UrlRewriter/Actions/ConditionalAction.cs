// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

namespace YAF.UrlRewriter.Actions;

using System;
using System.Collections.Generic;
using System.Linq;

using YAF.UrlRewriter.Conditions;
using YAF.UrlRewriter.Extensions;

/// <summary>
/// A Conditional Action
/// </summary>
public class ConditionalAction : IRewriteAction, IRewriteCondition
{
    /// <summary>
    /// Conditions that must hold for the rule to fire.
    /// </summary>
    public IList<IRewriteCondition> Conditions { get; } = new List<IRewriteCondition>();

    /// <summary>
    /// Child rules.
    /// </summary>
    public IList<IRewriteAction> Actions { get; } = new List<IRewriteAction>();

    /// <summary>
    /// Determines if the action matches the current context.
    /// </summary>
    /// <param name="context">The context to match on.</param>
    /// <returns>True if the condition matches.</returns>
    public virtual bool IsMatch(IRewriteContext context)
    {
        return this.Conditions.IsMatch(context);
    }

    /// <summary>
    /// Executes the rule.
    /// </summary>
    /// <param name="context">The rewrite context</param>
    public virtual RewriteProcessing Execute(IRewriteContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // Execute the actions.
        return (from t in this.Actions
                let condition = t as IRewriteCondition
                where condition == null || condition.IsMatch(context)
                select t
                into action
                select action.Execute(context))
            .FirstOrDefault(processing => processing != RewriteProcessing.ContinueProcessing);
    }
}